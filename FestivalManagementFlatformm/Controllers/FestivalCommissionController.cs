using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/festival-commissions")]
    public class FestivalCommissionController : Controller
    {
        private readonly IFestivalCommissionService _service;

        public FestivalCommissionController(IFestivalCommissionService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FestivalCommissionCreateRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("update/{commissionId}")]
        public async Task<IActionResult> Update(int commissionId, [FromQuery] decimal? amount, [FromQuery] double? commissionRate)
        {
            var result = await _service.UpdateAsync(commissionId, amount, commissionRate);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
           [FromQuery] int? commissionId,
           [FromQuery] int? festivalId,
           [FromQuery] int? pageNumber,
           [FromQuery] int? pageSize)
        {
            try
            {
                var commissions = await _service.SearchFestivalCommissionsAsync(
                    commissionId, festivalId, pageNumber, pageSize);

                return Ok(commissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm commission", detail = ex.Message });
            }
        }


        [HttpDelete("delete/{commissionId}")]
        public async Task<IActionResult> Delete(int commissionId)
        {
            await _service.DeleteAsync(commissionId);
            return Ok(new { success = true, message = "Xóa FestivalCommission thành công" });
        }

    }
}
