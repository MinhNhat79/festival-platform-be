using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/menuitems")]
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MenuItemCreateRequest request)
        {
            var result = await _menuItemService.CreateMenuItemAsync(request);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
           [FromQuery] int? itemId,
           [FromQuery] int? menuId,
           [FromQuery] string? itemName,
           [FromQuery] string? itemType,
           [FromQuery] int? pageNumber,
           [FromQuery] int? pageSize)
        {
            var items = await _menuItemService.SearchMenuItemsAsync(itemId, menuId, itemName, itemType, pageNumber, pageSize);
            return Ok(items);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
           int itemId,
           [FromQuery] int menuId,
           [FromQuery] string itemName,
           [FromQuery] string? description,
           [FromQuery] string itemType,
           [FromQuery] decimal basePrice)
        {
            var item = await _menuItemService.UpdateMenuItemAsync(itemId, menuId, itemName, description, itemType, basePrice);
            return Ok(item);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int itemId)
        {
            await _menuItemService.DeleteMenuItemAsync(itemId);
            return NoContent();
        }
    }
}
