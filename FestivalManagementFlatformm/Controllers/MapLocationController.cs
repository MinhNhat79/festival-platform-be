using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/maplocations")]
    public class MapLocationController : ControllerBase
    {
        private readonly IMapLocationService _maplocationservice;
        public MapLocationController(IMapLocationService maplocationservice)
        {
            _maplocationservice = maplocationservice;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMapLocation([FromBody] MapLocationCreateRequest request)
        {
            try
            {
                var result = await _maplocationservice.CreateMapLocationAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            int id,
            [FromQuery] string? locationName,
            [FromQuery] string? locationType,
            [FromQuery] string? coordinates,
            [FromQuery] bool? isOccupied)
        {
            try
            {
                var result = await _maplocationservice.UpdateMapLocationAsync(id, locationName, locationType, coordinates, isOccupied);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? locationId,
            [FromQuery] int? mapId,
            [FromQuery] string? locationName,
            [FromQuery] string? locationType,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _maplocationservice.SearchMapLocationsAsync(locationId, mapId, locationName, locationType, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _maplocationservice.DeleteMapLocationAsync(id);
                if (success)
                {
                    return Ok(new { success = true, message = "Xóa thành công" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Không tìm thấy bản ghi để xóa" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
