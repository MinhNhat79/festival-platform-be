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

        [HttpPost]
        public async Task<IActionResult> Create(FestivalSchoolCreateRequest request)
        {
            var result = await _service.CreateFestivalSchoolAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, int? festivalId, int? schoolId)
        {
            var result = await _service.UpdateFestivalSchoolAsync(id, festivalId, schoolId);
            return Ok(result);
        }


        [HttpPut("update/reject")]
        public async Task<IActionResult> Rejected(int id)
        {
            await _service.UpdateFestivalSchoolStatusToRejectAsync(id);
            return Ok("Cập nhật trạng thái 'reject' thành công");
        }
        [HttpPut("update/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            await _service.UpdateFestivalSchoolStatusToApproveAsync(id);
            return Ok("Cập nhật trạng thái 'approve' thành công");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? festivalSchoolId, int? festivalId, int? schoolId, string? status, int? pageNumber, int? pageSize)
        {
            var result = await _service.SearchFestivalSchoolsAsync(festivalSchoolId, festivalId, schoolId, status, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteFestivalSchoolAsync(id);
            return NoContent();
        }

    }
}
