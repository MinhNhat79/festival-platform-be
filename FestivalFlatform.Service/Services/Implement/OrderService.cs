using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public OrderService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            

            // Check account exists
            var accountExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);
            if (!accountExists)
                throw new ArgumentException("Account does not exist");

            // Check booth exists
            var boothExists = await _unitOfWork.Repository<Booth>()
                .AnyAsync(b => b.BoothId == request.BoothId);
            if (!boothExists)
                throw new ArgumentException("Booth does not exist");

          

            var newOrder = new Order
            {
                AccountId = request.AccountId,
                BoothId = request.BoothId,
                TotalAmount = request.TotalAmount,
                PointsUsed = request.PointsUsed,
                CashAmount = request.CashAmount,
                Notes = request.Notes,
                Status = StatusOrder.Pending,
                OrderDate = DateTime.UtcNow,

            };

            await _unitOfWork.Repository<Order>().InsertAsync(newOrder);
            await _unitOfWork.CommitAsync();

            return newOrder;
        }

        public async Task<Order> UpdateOrderAsync(
        int orderId,
        decimal? totalAmount,
        int? pointsUsed,
        decimal? cashAmount,
        string? notes,
        string? status)
        {
            var order = await _unitOfWork.Repository<Order>().GetAll()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", orderId.ToString());

            if (totalAmount.HasValue)
                order.TotalAmount = totalAmount.Value;

            if (pointsUsed.HasValue)
                order.PointsUsed = pointsUsed.Value;

            if (cashAmount.HasValue)
                order.CashAmount = cashAmount.Value;

            if (!string.IsNullOrWhiteSpace(notes))
                order.Notes = notes.Trim();

            if (!string.IsNullOrWhiteSpace(status))
                order.Status = status.Trim().ToLower();

            order.OrderDate = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return order;
        }

        public async Task<List<Order>> SearchOrdersAsync(
       int? orderId,
       int? accountId,
       int? boothId,
       string? status,
       DateTime? fromDate,
       DateTime? toDate,
       int? pageNumber,
       int? pageSize)
        {
            var query = _unitOfWork.Repository<Order>().GetAll()
                .Where(o => !orderId.HasValue || o.OrderId == orderId.Value)
                .Where(o => !accountId.HasValue || o.AccountId == accountId.Value)
                .Where(o => !boothId.HasValue || o.BoothId == boothId.Value)
                .Where(o => string.IsNullOrWhiteSpace(status) || o.Status == status.Trim().ToLower())
                .Where(o => !fromDate.HasValue || o.OrderDate >= fromDate.Value)
                .Where(o => !toDate.HasValue || o.OrderDate <= toDate.Value);

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            query = query
                .Skip((currentPage - 1) * currentSize)
                .Take(currentSize);

            var result = await query.ToListAsync();

           

            return result;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetAll()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Order", orderId.ToString());
            }

            _unitOfWork.Repository<Order>().Delete(order);
            await _unitOfWork.CommitAsync();
        }



    }
}
