using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/account-points")]
    public class AccountPointsController : Controller
    {

        private readonly IAccountPointsService _accountPointsService;
        public AccountPointsController(IAccountPointsService accountPointsService)
        {
            _accountPointsService = accountPointsService;
        }



        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountPoints(int accountId)
        {
            var result = await _accountPointsService.CreateAccountPointsAsync(accountId);
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAccountPoints([FromQuery] int? accountPointsId, [FromQuery] int? accountId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _accountPointsService.SearchAccountPointsAsync(accountPointsId, accountId,  pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePointsBalance(int accountPointsId, [FromBody] int newPointsBalance)
        {
            try
            {
                var result = await _accountPointsService.UpdateAccountPointsAsync(accountPointsId, newPointsBalance);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật điểm", detail = ex.Message });
            }
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccountPoints(int accountPointsId)
        {
            try
            {
                await _accountPointsService.DeleteAccountPointsAsync(accountPointsId);
                return NoContent(); // 204 No Content
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa điểm tài khoản", detail = ex.Message });
            }
        }

    }

}
