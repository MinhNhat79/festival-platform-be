using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IOrderItemService
    {
        Task<OrderItem> CreateOrderItemAsync(OrderItemCreateRequest request);
        Task<OrderItem> UpdateOrderItemAsync(int id, int quantity, decimal unitPrice);
        Task<List<OrderItem>> SearchOrderItemsAsync(int? orderItemId, int? orderId, int? menuItemId, int? page, int? size);
        Task DeleteOrderItemAsync(int id);
    }
}
