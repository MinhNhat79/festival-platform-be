using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FestivalManagementFlatformm.Controllers
{
    [Route("api/orderitems")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _service;

        public OrderItemController(IOrderItemService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] OrderItemCreateRequest request)
        {
            try
            {
                var result = await _service.CreateOrderItemAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, int quantity, decimal unitPrice)
        {
            try
            {
                var result = await _service.UpdateOrderItemAsync(id, quantity, unitPrice);
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
        public async Task<IActionResult> Search(int? orderItemId, int? orderId, int? menuItemId, int? page, int? size)
        {
            try
            {
                var result = await _service.SearchOrderItemsAsync(orderItemId, orderId, menuItemId, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                 await _service.DeleteOrderItemAsync(id);
               

                return Ok(new { Success = true, Message = "Deleted successfully" });
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
