using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : Controller
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest request)
        {
            var result = await _service.CreateWalletAsync(request);
            return Ok(result);
        }

    
        [HttpPut("update")]
        public async Task<IActionResult> UpdateWallet(int walletId, decimal balance)
        {
            var result = await _service.UpdateWalletAsync(walletId, balance);
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(int? userId, int? pageNumber, int? pageSize)
        {
            var result = await _service.SearchWalletsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteWallet(int walletId)
        {
            var result = await _service.DeleteWalletAsync(walletId);
            return Ok(new { success = result });
        }
    }
}
