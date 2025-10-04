using FestivalFlatform.Data.Models;
using System.Net;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/festivals")]
    public class FestivalController : Controller
    {
        private readonly IFestivalService _festivalService;

        public FestivalController(IFestivalService festivalService)
        {
            _festivalService = festivalService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFestival([FromBody] FestivalCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });

            try
            {
                var createdFestival = await _festivalService.CreateFestivalAsync(request);
                return Ok(createdFestival);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi khi tạo festival", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateFestival(
            int id,
            [FromQuery] int? maxFoodBooths,
            [FromQuery] int? maxBeverageBooths,
            [FromQuery] int? registeredFoodBooths,
            [FromQuery] int? registeredBeverageBooths,
            [FromQuery] string? cancelReason,
            [FromQuery] string? status)
        {
            try
            {
                var result = await _festivalService.UpdateFestivalAsync(id, maxFoodBooths, maxBeverageBooths, registeredFoodBooths, registeredBeverageBooths, cancelReason, status);
                if (result == null)
                    return NotFound(new { success = false, message = "Festival không tồn tại" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi khi cập nhật festival", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFestivals(
            [FromQuery] int? festivalId,
            [FromQuery] int? schoolId,
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] DateTime? registrationStartDate,
            [FromQuery] DateTime? registrationEndDate,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _festivalService.SearchFestivalsAsync(
                    festivalId, schoolId, status, startDate, endDate, registrationStartDate, registrationEndDate, pageNumber, pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm festival", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFestival(int id)
        {
            try
            {
                await _festivalService.DeleteFestivalAsync(id);
                return Ok(new { success = true, message = "Xoá thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Festival không tồn tại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xoá festival", detail = ex.Message });
            }
        }
        [HttpPost("calculate-commission")]
        public async Task<IActionResult> CalculateCommission(
     [FromBody] DistributeCommissionRequest request)
        {
            try
            {
                
               

                await _festivalService.DistributeCommissionAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Tính và phân bổ hoa hồng thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
     
        public async Task<IActionResult> GetFestivalDetail(int id)
        {
            var festival = await _festivalService.GetFestivalDetailAsync(id);
            return Ok(festival);
        }



        [HttpPut("update-info")]
        public async Task<IActionResult> UpdateFestivalInfo([FromBody] UpdateFestivalRequest request)
        {
            try
            {
                var result = await _festivalService.UpdateFestivalInfoAsync(request);
                return Ok(new { success = true, data = result });
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

        [HttpDelete("IsDelete")]
        public async Task<IActionResult> SoftDeleteFestival(int id)
        {
            var success = await _festivalService.SoftDeleteFestivalAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy festival" });

            return Ok(new { message = "Đã xoá festival thành công" });
        }

        [HttpGet("can-update-ongoing")]
        public async Task<IActionResult> CanUpdateOngoingFestival(int festivalId)
        {
            var result = await _festivalService.CanUpdateOngoingFestivalAsync(festivalId);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = "Có thể chuyển festival sang trạng thái Ongoing" });
        }
    }
}
