using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/festivalmaps")]
    public class FestivalMapController : Controller
    {
        private readonly IFestivalMapService _festivalmapService;

        public FestivalMapController(IFestivalMapService festivalmapService)
        {
            _festivalmapService = festivalmapService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFestivalMap([FromBody] FestivalMapCreateRequest request)
        {
            try
            {
                var result = await _festivalmapService.CreateFestivalMapAsync(request);
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
        public async Task<IActionResult> UpdateFestivalMap(
            int mapId,
            [FromQuery] int? festivalId,
            [FromQuery] string? mapName,
            [FromQuery] string? mapType,
            [FromQuery] string? mapUrl)
        {
            try
            {
                var result = await _festivalmapService.UpdateFestivalMapAsync(mapId, festivalId, mapName, mapType, mapUrl);
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
        public async Task<IActionResult> SearchFestivalMaps(
            [FromQuery] int? mapId,
            [FromQuery] int? festivalId,
            [FromQuery] string? mapName,
            [FromQuery] string? mapType,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _festivalmapService.SearchFestivalMapsAsync(mapId, festivalId, mapName, mapType, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFestivalMap(int mapId)
        {
            try
            {
                var result = await _festivalmapService.DeleteFestivalMapAsync(mapId);
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
