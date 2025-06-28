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
            var result = await _service.CreateChatSessionAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int sessionId, DateTime? lastMessageAt)
        {
            var result = await _service.UpdateChatSessionAsync(sessionId, lastMessageAt);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            int? sessionId, int? accountId,
            int? pageNumber, int? pageSize)
        {
            var result = await _service.SearchChatSessionsAsync(sessionId, accountId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int sessionId)
        {
            var result = await _service.DeleteChatSessionAsync(sessionId);
            return Ok(new { success = result });
        }
    }
}
