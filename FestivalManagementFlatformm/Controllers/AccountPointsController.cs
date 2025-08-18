using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/accountpoints")]
    public class AccountPointsController : Controller
    {
        private readonly IAccountPointsService _accountPointsService;
        public AccountPointsController(IAccountPointsService accountPointsService)
        {
            _accountPointsService = accountPointsService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountPoints([FromBody] CreateAccountPointsRequest request)
        {
            try
            {
                var result = await _accountPointsService.CreateAccountPointsAsync(request);
                return Ok(result);
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
        public async Task<IActionResult> SearchAccountPoints([FromQuery] int? accountPointsId, [FromQuery] int? accountId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _accountPointsService.SearchAccountPointsAsync(accountPointsId, accountId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePointsBalance(int accountPointsId, [FromBody] int newPointsBalance)
        {
            try
            {
                var result = await _accountPointsService.UpdateAccountPointsAsync(accountPointsId, newPointsBalance);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy tài khoản điểm." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccountPoints(int accountPointsId)
        {
            try
            {
                await _accountPointsService.DeleteAccountPointsAsync(accountPointsId);
                return Ok(new { success = true, message = "Xóa thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy tài khoản điểm cần xóa." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
