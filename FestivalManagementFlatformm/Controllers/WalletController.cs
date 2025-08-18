using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var result = await _service.CreateWalletAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateWallet(int walletId, decimal balance)
        {
            try
            {
                var result = await _service.UpdateWalletAsync(walletId, balance);
                if (result == null)
                {
                    return BadRequest(new { Success = false, Message = "Update failed." });
                }
                return Ok(new { Success = true, Message = "Update successful." });
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
        public async Task<IActionResult> Search(int? userId, int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _service.SearchWalletsAsync(userId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteWallet(int walletId)
        {
            try
            {
                var result = await _service.DeleteWalletAsync(walletId);
                if (!result)
                {
                    return NotFound(new { Success = false, Message = "Wallet not found." });
                }
                return Ok(new { Success = true, Message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
