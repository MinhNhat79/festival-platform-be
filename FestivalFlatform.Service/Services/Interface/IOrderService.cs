using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderRequest request);
        Task<Order> UpdateOrderAsync(
       int orderId,
       decimal? totalAmount,
       int? pointsUsed,
       decimal? cashAmount,
       string? notes,
       string? status);
        Task<List<Order>> SearchOrdersAsync(
       int? orderId,
       int? accountId,
       int? boothId,
       string? status,
       DateTime? fromDate,
       DateTime? toDate,
       int? pageNumber,
       int? pageSize);
        Task DeleteOrderAsync(int orderId);
    }
}
