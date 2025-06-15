using System.Drawing.Printing;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/maplocations")]
    public class MapLocationController : Controller
    {
        private readonly IMapLocationService _maplocationservice;
        public MapLocationController(IMapLocationService maplocationservice)
        {
            _maplocationservice = maplocationservice;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMapLocation([FromBody] MapLocationCreateRequest request)
        {
            var result = await _maplocationservice.CreateMapLocationAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            int id,
            [FromQuery] string? locationName,
            [FromQuery] string? locationType,
            [FromQuery] string? coordinates,
            [FromQuery] bool? isOccupied)
        {
            var result = await _maplocationservice.UpdateMapLocationAsync(id, locationName, locationType, coordinates, isOccupied);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
           [FromQuery] int? locationId,
           [FromQuery] int? mapId,
           [FromQuery] string? locationName,
           [FromQuery] string? locationType, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _maplocationservice.SearchMapLocationsAsync(locationId, mapId, locationName, locationType,  pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _maplocationservice.DeleteMapLocationAsync(id);
            return Ok(new { success });
        }
    }
}
