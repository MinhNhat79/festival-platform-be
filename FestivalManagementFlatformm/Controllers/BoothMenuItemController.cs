using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/boothmenuitems")]
    public class BoothMenuItemController : Controller
    {
        private readonly IBoothMenuItemService _boothmenuitemservice;

        public BoothMenuItemController(IBoothMenuItemService boothmenuitemservice)
        {
            _boothmenuitemservice = boothmenuitemservice;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BoothMenuItemCreateRequest request)
        {
            try
            {
                var result = await _boothmenuitemservice.CreateBoothMenuItemAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Tạo thất bại", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, decimal? customPrice, int? quantityLimit, int? remainingQuantity, string? status)
        {
            try
            {
                var result = await _boothmenuitemservice.UpdateBoothMenuItemAsync(id, customPrice, quantityLimit, remainingQuantity, status);
                if (result == null)
                    return NotFound(new { success = false, message = "Không tìm thấy booth menu item" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Cập nhật thất bại", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? boothMenuItemId, int? boothId, int? menuItemId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _boothmenuitemservice.SearchBoothMenuItemsAsync(boothMenuItemId, boothId, menuItemId, status, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lấy dữ liệu thất bại", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _boothmenuitemservice.DeleteBoothMenuItemAsync(id);
                return Ok(new { success = true, message = "Xóa thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy booth menu item" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Xóa thất bại", detail = ex.Message });
            }
        }
    }
}
