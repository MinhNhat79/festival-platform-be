using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/oders")]
    public class OrderController : Controller
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var result = await _service.CreateOrderAsync(request);
            return Ok(result);
           
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrder(
          int orderId,
          decimal? totalAmount,
          int? pointsUsed,
          decimal? cashAmount,
          string? notes,
          string? status)
        {
            var result = await _service.UpdateOrderAsync(
                orderId, totalAmount, pointsUsed, cashAmount, notes, status);
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders(
           int? orderId,
           int? accountId,
           int? boothId,
           string? status,
           DateTime? fromDate,
           DateTime? toDate,
           int? pageNumber,
           int? pageSize)
        {
            var result = await _service.SearchOrdersAsync(
                orderId, accountId, boothId, status, fromDate, toDate, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            await _service.DeleteOrderAsync(orderId);
            return Ok(new { message = "Xóa order thành công" });
        }
    }
}
