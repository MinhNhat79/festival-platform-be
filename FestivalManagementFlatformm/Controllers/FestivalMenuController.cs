using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/festivalmenus")]
    public class FestivalMenuController : Controller
    {
        private readonly IFestivalMenuService _festivalMenuService;

        public FestivalMenuController(IFestivalMenuService festivalMenuService)
        {
            _festivalMenuService = festivalMenuService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFestivalMenu([FromBody] FestivalMenuCreateRequest request)
        {
            var result = await _festivalMenuService.CreateFestivalMenuAsync(request);
            return Ok(result);
        }

      
        [HttpPut("update")]
        public async Task<IActionResult> UpdateFestivalMenu(int menuId, [FromQuery] string? menuName, [FromQuery] string? description)
        {
            var result = await _festivalMenuService.UpdateFestivalMenuAsync(menuId, menuName, description);
            return Ok(result);
        }

        // GET: api/festival-menus/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchFestivalMenus([FromQuery] int? menuId, [FromQuery] int? festivalId, [FromQuery] string? menuName, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _festivalMenuService.SearchFestivalMenusAsync(menuId, festivalId, menuName, pageNumber, pageSize);
            return Ok(result);
        }

        // DELETE: api/festival-menus/{menuId}
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFestivalMenu(int menuId)
        {
            var result = await _festivalMenuService.DeleteFestivalMenuAsync(menuId);
            return Ok(new { success = result });
        }
    }
}
