using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/festivalparticipants")]
    public class FestivalParticipantsController : Controller
    {
        private readonly IFestivalParticipantService _service;

        public FestivalParticipantsController(IFestivalParticipantService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FestivalParticipantCreateRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
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
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tạo participant", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFestivalParticipants(
     [FromQuery] int? participantId,
     [FromQuery] int? festivalId,
     [FromQuery] int? accountId,
     [FromQuery] int? pageNumber,
     [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _service.SearchFestivalParticipantsAsync(
                    participantId, festivalId, accountId, pageNumber, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Lỗi khi tìm kiếm FestivalParticipants",
                    detail = ex.Message
                });
            }
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] FestivalParticipantCreateRequest request)
        {
            try
            {
                var result = await _service.DeleteAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi xoá participant", detail = ex.Message });
            }
        }
    }
}
