using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
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
            var result = await _festivalmapService.CreateFestivalMapAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateFestivalMap(int mapId, [FromQuery] int? festivalId, [FromQuery] string? mapName, [FromQuery] string? mapType, [FromQuery] string? mapUrl)
        {
            var result = await _festivalmapService.UpdateFestivalMapAsync(mapId, festivalId, mapName, mapType, mapUrl);
            return Ok(result);

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
            var result = await _festivalmapService.SearchFestivalMapsAsync(mapId, festivalId, mapName, mapType, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFestivalMap(int mapId)
        {
            await _festivalmapService.DeleteFestivalMapAsync(mapId);
            return NoContent();
        }
    }
}
