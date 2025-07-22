using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IPaymentService
    {
        Task<bool> DeletePaymentAsync(int id);
        Task<List<Payment>> SearchPaymentsAsync(
       int? orderId,
       int? walletId,
       string? paymentMethod,
       string? paymentType,
       string? status,
       int? pageNumber,
       int? pageSize);
        Task<Payment> UpdatePaymentAsync(int id, string status, string? description);
        Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequest request);
        Task<bool> HandleWebhookAsync(string rawJson);
    }
}
