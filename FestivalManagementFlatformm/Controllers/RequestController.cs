using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/requests")]
    public class RequestController : Controller
    {
        private readonly IRequestService _service;

        public RequestController(IRequestService service)
        {
            _service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RequestCreateRequest request)
        {
            try
            {
                var result = await _service.CreateRequestAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update(
        int requestId,
        string? status = null,
        string? message = null,
        string? type = null)
        {
            try
            {
                var result = await _service.UpdateRequestAsync(requestId, status, message, type);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            int? requestId, int? accountId, string? status, string? type,
            int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _service.SearchRequestsAsync(requestId, accountId, status, type, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteRequestAsync(id);
                if (!success)
                    return BadRequest(new { Success = false, Message = "Delete failed" });

                return Ok(new { Success = true, Message = "Deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
