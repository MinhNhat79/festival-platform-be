using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/menuitems")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MenuItemCreateRequest request)
        {
            try
            {
                var result = await _menuItemService.CreateMenuItemAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
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
            try
            {
                var items = await _menuItemService.SearchMenuItemsAsync(itemId, menuId, itemName, itemType, pageNumber, pageSize);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            int itemId,
            [FromQuery] int menuId,
            [FromQuery] string itemName,
            [FromQuery] string? description,
            [FromQuery] string itemType,
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice)
        {
            try
            {
                var item = await _menuItemService.UpdateMenuItemAsync(itemId, menuId, itemName, description, itemType, minPrice, maxPrice);
                return Ok(item);
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

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int itemId)
        {
            try
            {
                await _menuItemService.DeleteMenuItemAsync(itemId);
                return Ok(new { Success = true, Message = "Xóa thành công" });
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
