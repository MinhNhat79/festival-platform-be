using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/accountwallethistories")]
    public class AccountWalletHistoryController : Controller
    {
        private readonly IAccountWalletHistoryService _service;
        public AccountWalletHistoryController(IAccountWalletHistoryService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAccountWalletHistoryRequest request)
        {
            try
            {
                var result = await _service.CreateHistoryAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] int historyId, [FromQuery] string? description)
        {
            try
            {
                var result = await _service.UpdateHistoryAsync(historyId, description);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lịch sử ví." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? accountId,
            [FromQuery] int? historyId,
            [FromQuery] string? type,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _service.SearchHistoriesAsync(accountId, historyId, type, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int historyId)
        {
            try
            {
                await _service.DeleteHistoryAsync(historyId);
                return Ok(new { success = true, message = "Xóa thành công." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lịch sử ví để xóa." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }
    }
}
