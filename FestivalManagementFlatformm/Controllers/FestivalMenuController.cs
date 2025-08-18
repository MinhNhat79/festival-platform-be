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
            try
            {
                var result = await _festivalMenuService.CreateFestivalMenuAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateFestivalMenu(
            int menuId,
            [FromQuery] string? menuName,
            [FromQuery] string? description)
        {
            try
            {
                var result = await _festivalMenuService.UpdateFestivalMenuAsync(menuId, menuName, description);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFestivalMenus(
            [FromQuery] int? menuId,
            [FromQuery] int? festivalId,
            [FromQuery] string? menuName,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _festivalMenuService.SearchFestivalMenusAsync(menuId, festivalId, menuName, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFestivalMenu(int menuId)
        {
            try
            {
                var result = await _festivalMenuService.DeleteFestivalMenuAsync(menuId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
    }
}
