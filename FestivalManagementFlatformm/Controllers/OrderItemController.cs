using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _service.CreateOrderItemAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, int quantity, decimal unitPrice)
        {
            var result = await _service.UpdateOrderItemAsync(id, quantity, unitPrice);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? orderItemId, int? orderId, int? menuItemId, int? page, int? size)
        {
            var result = await _service.SearchOrderItemsAsync(orderItemId, orderId, menuItemId, page, size);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteOrderItemAsync(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }

}
