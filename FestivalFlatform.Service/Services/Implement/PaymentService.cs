using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;


namespace FestivalFlatform.Service.Services.Implement
{
    public class PaymentService : IPaymentService
    {
        PayOS _payOS;
        private readonly ILogger<PaymentService> _logger;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public PaymentService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration config, PayOS payOS, ILogger<PaymentService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = config;
            _payOS = payOS;
            _logger = logger;
        }



        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                throw new ArgumentException("Phương thức thanh toán là bắt buộc");

            if (string.IsNullOrWhiteSpace(request.PaymentType))
                throw new ArgumentException("Loại giao dịch là bắt buộc");

            var paymentType = request.PaymentType.Trim().ToLower();
            long orderCode;
            string description;

            if (paymentType == "order")
            {
                if (!request.OrderId.HasValue)
                    throw new ArgumentException("OrderId là bắt buộc với loại giao dịch order");

                var orderExists = await _unitOfWork.Repository<Order>()
                    .AnyAsync(o => o.OrderId == request.OrderId.Value);
             

                orderCode = request.OrderId.Value;
                description = request.Description ?? $"Thanh toán đơn hàng #{orderCode}";
            }
            else
            {
                if (!request.WalletId.HasValue)
                    throw new ArgumentException("WalletId là bắt buộc với loại giao dịch ví");

                var walletExists = await _unitOfWork.Repository<Wallet>()
                    .AnyAsync(w => w.WalletId == request.WalletId.Value);
                if (!walletExists)
                    throw new ArgumentException("Không tìm thấy ví tương ứng");

                orderCode = request.WalletId.Value;
                description = request.Description ?? $"Nạp ví #{orderCode}";
            }

            var payment = new Payment
            {
                OrderId = request.OrderId,
                WalletId = request.WalletId,
                PaymentMethod = request.PaymentMethod.Trim(),
                PaymentType = request.PaymentType.Trim(),
                AmountPaid = request.AmountPaid,
                Status = StatusPayment.Pending,
                Description = description,
                PaymentDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Payment>().InsertAsync(payment);
            await _unitOfWork.CommitAsync();

            // Gửi lên PayOS
            var itemList = new List<ItemData>();

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)request.AmountPaid,
                description: description,
                items: itemList,
                cancelUrl: _config["PayOS:ReturnUrl"],
                returnUrl: _config["PayOS:CancelUrl"],
                null, null, null, null, null, null

            );

            CreatePaymentResult createPayment;

            try
            {
                createPayment = await _payOS.createPaymentLink(paymentData);
            }
            catch (Exception ex)
            {
                await _unitOfWork.Repository<Payment>().DeleteAsync(payment);
                await _unitOfWork.CommitAsync();
                throw new InvalidOperationException("Không thể tạo link thanh toán từ PayOS", ex);
            }

            
            return new PaymentResponseDto
            {
                OrderCode = paymentData.orderCode,
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                WalletId = payment.WalletId,
                PaymentMethod = payment.PaymentMethod,
                PaymentType = payment.PaymentType,
                AmountPaid = payment.AmountPaid,
                Status = payment.Status,
                Description = payment.Description,
                PaymentDate = payment.PaymentDate,
                CheckoutUrl = createPayment.checkoutUrl
            };
        }

        public async Task<bool> HandleWebhookAsync(string rawJson)
        {
            dynamic payload;
            try
            {
                payload = JsonSerializer.Deserialize<dynamic>(rawJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Không thể parse JSON webhook.");
                return false;
            }

            string? code = payload?.code;
            string? status = payload?.status;
            long? orderCode = payload?.orderCode;
            long? amount = payload?.amount;
            string? description = payload?.description;

            if (payload?.data != null)
            {
                orderCode ??= payload.data.orderCode;
                amount ??= payload.data.amount;
                description ??= payload.data.description;
            }

            if (code == "00" && status == "PAID")
            {
                if (orderCode == null || amount == null)
                {
                    _logger.LogWarning("❌ orderCode hoặc amount bị null.");
                    return false;
                }

                _logger.LogInformation($"✅ Giao dịch thành công: orderCode={orderCode}, amount={amount}, desc={description}");

                if (!string.IsNullOrEmpty(description) && description.Contains("Hoa don", StringComparison.OrdinalIgnoreCase))
                {
                    // 👉 orderCode là OrderId
                    var order = await _unitOfWork.Repository<Order>().GetAll().FirstOrDefaultAsync(x => x.OrderId == orderCode);
                    if (order == null)
                    {
                        _logger.LogWarning($"❌ Không tìm thấy Order với OrderId={orderCode}");
                        return false;
                    }

                    order.Status = "Completed";

                    var payment = await _unitOfWork.Repository<Payment>().GetAll()
                        .FirstOrDefaultAsync(p => p.OrderId == order.OrderId);

                    if (payment != null)
                    {
                        payment.Status = StatusPayment.Success;
                        payment.PaymentDate = DateTime.UtcNow;
                    }

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("✅ Đã cập nhật Order và Payment.");
                }
                else
                {
                    // 👉 orderCode là WalletId
                    var wallet = await _unitOfWork.Repository<Wallet>().GetAll().FirstOrDefaultAsync(x => x.WalletId == orderCode);
                    if (wallet == null)
                    {
                        _logger.LogWarning($"❌ Không tìm thấy Wallet với WalletId={orderCode}");
                        return false;
                    }

                    wallet.Balance += amount.Value;

                    var payment = await _unitOfWork.Repository<Payment>().GetAll()
                        .FirstOrDefaultAsync(p => p.WalletId == wallet.WalletId);

                    if (payment != null)
                    {
                        payment.Status = StatusPayment.Success;
                        payment.PaymentDate = DateTime.UtcNow;
                    }

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("✅ Đã nạp tiền vào ví và cập nhật Payment.");
                }

                return true;
            }
            else
            {
                _logger.LogWarning($"❌ Giao dịch thất bại hoặc không hợp lệ: code={code}, status={status}");

                if (orderCode != null)
                {
                    var payment = await _unitOfWork.Repository<Payment>().GetAll()
                        .FirstOrDefaultAsync(p => p.OrderId == orderCode || p.WalletId == orderCode);

                    if (payment != null)
                    {
                        payment.Status = StatusPayment.Failed;
                        await _unitOfWork.SaveChangesAsync();
                        _logger.LogInformation($"⚠️ Đã đánh dấu Payment thất bại với ID={payment.PaymentId}");
                    }
                }

                return false;
            }
        }
        // hàm  handle payos webhook cho bên thứ 3 call vô => tạo API
        // => sửa lý để lấy data từ body truyền vào
        //sửa lý khúc    if (payload != null && payload.code =="00" || payload.status == "PAID" ) lồng thêm 1 if {payload.description contains=="Hoa don" thì ordercode là orderid} ngược lại ordercode sẽ là walletid 
        //nếu là orderid thì sẽ chỏ vô bảng order kiếm cái order nào có id trùng đó thì chuyển status thành completed  sau đó dựa vô dùng ordercode để so sánh với orderid trong bảng payment rồi chuyển status payment thành success
        //nếu là wallet thì tăng balance thành amount tương như như trên nhưng dùng ordercode để so sánh với walletid.


        public async Task<Payment> UpdatePaymentAsync(int id, string status, string? description)
        {
            var payment = await _unitOfWork.Repository<Payment>().GetAll().FirstOrDefaultAsync(p => p.PaymentId == id)
                ?? throw new Exception("Payment not found");

            payment.Status = status;
            payment.Description = description;
            await _unitOfWork.CommitAsync();

            return payment;
        }
        public async Task<List<Payment>> SearchPaymentsAsync(
      int? orderId,
      int? walletId,
      string? paymentMethod,
      string? paymentType,
      string? status,
      int? pageNumber,
      int? pageSize)
        {
            var query = _unitOfWork.Repository<Payment>().GetAll()
                .Where(p => !orderId.HasValue || p.OrderId == orderId)
                .Where(p => !walletId.HasValue || p.WalletId == walletId)
                .Where(p => string.IsNullOrWhiteSpace(paymentMethod) || p.PaymentMethod == paymentMethod.Trim())
                .Where(p => string.IsNullOrWhiteSpace(paymentType) || p.PaymentType == paymentType.Trim())
                .Where(p => string.IsNullOrWhiteSpace(status) || p.Status == status.Trim());

            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);

            query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            return await query.ToListAsync();
        }
        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _unitOfWork.Repository<Payment>().GetAll().FirstOrDefaultAsync(p => p.PaymentId == id)
                ?? throw new Exception("Payment not found");

            _unitOfWork.Repository<Payment>().Delete(payment);
            await _unitOfWork.CommitAsync();

            return true;
        }

    }

}
