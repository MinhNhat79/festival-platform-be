using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/festivalschools")]
    public class FestivalSchoolController : Controller
    {
        private readonly IFestivalSchoolService _service;

        public FestivalSchoolController(IFestivalSchoolService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FestivalSchoolCreateRequest request)
        {
            try
            {
                var result = await _service.CreateFestivalSchoolAsync(request);
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
        public async Task<IActionResult> Update(int id, int? festivalId, int? schoolId)
        {
            try
            {
                var result = await _service.UpdateFestivalSchoolAsync(id, festivalId, schoolId);
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

        [HttpPut("update/reject")]
        public async Task<IActionResult> Rejected(int id, string? rejectReason)
        {
            try
            {
                await _service.UpdateFestivalSchoolStatusToRejectAsync(id, rejectReason);
                return Ok(new { success = true, message = "Cập nhật trạng thái 'reject' thành công" });
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

        [HttpPut("update/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _service.UpdateFestivalSchoolStatusToApproveAsync(id);
                return Ok(new { success = true, message = "Cập nhật trạng thái 'approve' thành công" });
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
        public async Task<IActionResult> Search(
            int? festivalSchoolId,
            int? festivalId,
            int? schoolId,
            string? status,
            int? pageNumber,
            int? pageSize)
        {
            try
            {
                var result = await _service.SearchFestivalSchoolsAsync(
                    festivalSchoolId, festivalId, schoolId, status, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteFestivalSchoolAsync(id);
                return Ok(new { success = true });
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
