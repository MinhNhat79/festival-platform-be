using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var result = await _service.CreateOrderAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
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
            try
            {
                var result = await _service.UpdateOrderAsync(orderId, totalAmount, pointsUsed, cashAmount, notes, status);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
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
            try
            {
                var result = await _service.SearchOrdersAsync(orderId, accountId, boothId, status, fromDate, toDate, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                await _service.DeleteOrderAsync(orderId);
                return Ok(new { Success = true, Message = "Xóa order thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
