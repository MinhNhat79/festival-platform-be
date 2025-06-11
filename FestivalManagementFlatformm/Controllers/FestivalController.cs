using FestivalFlatform.Data.Models;
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
                return BadRequest(ModelState);

            try
            {
                var createdFestival = await _festivalService.CreateFestivalAsync(request);
                return Ok(createdFestival);
            }
            catch (Exception ex)
            {
                // log error nếu cần
                return StatusCode(500, $"Lỗi khi tạo festival: {ex.Message}");
            }


        }

        [HttpPut("update")]
        public async Task<ActionResult<Festival>> UpdateFestival(int id,
           [FromQuery] int? maxFoodBooths,
           [FromQuery] int? maxBeverageBooths,
           [FromQuery] int? registeredFoodBooths,
           [FromQuery] int? registeredBeverageBooths,
           [FromQuery] string? status)
        {
            var result = await _festivalService.UpdateFestivalAsync(id, maxFoodBooths, maxBeverageBooths, registeredFoodBooths, registeredBeverageBooths, status);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Festival>>> SearchFestivals(
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
            var result = await _festivalService.SearchFestivalsAsync(festivalId, schoolId, status, startDate, endDate, registrationStartDate, registrationEndDate, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFestival(int id)
        {
            await _festivalService.DeleteFestivalAsync(id);
            return NoContent();
        }
    }
}
