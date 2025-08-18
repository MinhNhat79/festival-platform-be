using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/accountfestivalwallets")]
    public class AccountFestivalWalletController : Controller
    {
        private readonly IAccountFestivalWalletService _service;
        public AccountFestivalWalletController(IAccountFestivalWalletService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AccountFestivalWalletCreateRequest request)
        {
            try
            {
                var result = await _service.CreateAccountFestivalWalletAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? accountFestivalWalletId,
            [FromQuery] int? accountId,
            [FromQuery] int? festivalId,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _service.SearchAccountFestivalWalletsAsync(
                    accountFestivalWalletId, accountId, festivalId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            [FromQuery] int id,
            [FromQuery] decimal newBalance,
            [FromQuery] string? newName)
        {
            try
            {
                var result = await _service.UpdateAccountFestivalWalletAsync(id, newBalance, newName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            try
            {
                await _service.DeleteAccountFestivalWalletAsync(id);
                return Ok(new { success = true, message = "Xóa thành công." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Không tìm thấy ví cần xóa." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("transfer-to-account-festival-wallet")]
        public async Task<IActionResult> TransferToAccountFestivalWallet([FromBody] WalletTransferRequest request)
        {
            try
            {
                await _service.TransferToAccountFestivalWalletAsync(request);
                return Ok(new { success = true, message = "Chuyển tiền thành công." });
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

        [HttpPost("transfer-to-main-wallet")]
        public async Task<IActionResult> TransferToMainWallet([FromBody] WalletTransferRequest request)
        {
            try
            {
                await _service.TransferToMainWalletAsync(request);
                return Ok(new { success = true, message = "Chuyển tiền thành công." });
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
    }
}
