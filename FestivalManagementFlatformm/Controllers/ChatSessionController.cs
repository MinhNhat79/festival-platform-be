using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/chatsessions")]
    public class ChatSessionController : Controller
    {
        private readonly IChatSessionService _service;

        public ChatSessionController(IChatSessionService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ChatSessionCreateRequest request)
        {
            try
            {
                var result = await _service.CreateChatSessionAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Tạo chat session thất bại", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int sessionId, DateTime? lastMessageAt)
        {
            try
            {
                var result = await _service.UpdateChatSessionAsync(sessionId, lastMessageAt);
                if (result == null)
                    return NotFound(new { success = false, message = "Không tìm thấy chat session" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Cập nhật thất bại", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? sessionId, int? accountId, int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _service.SearchChatSessionsAsync(sessionId, accountId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lấy danh sách thất bại", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int sessionId)
        {
            try
            {
                var result = await _service.DeleteChatSessionAsync(sessionId);
                if (!result)
                    return NotFound(new { success = false, message = "Không tìm thấy chat session" });

                return Ok(new { success = true, message = "Xóa thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Xóa thất bại", detail = ex.Message });
            }
        }
    }
}
