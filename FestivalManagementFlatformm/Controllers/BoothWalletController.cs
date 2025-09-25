using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/boothwallets")]
    public class BoothWalletController : Controller
    {

        private readonly IBoothWalletService _service;

        public BoothWalletController(IBoothWalletService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBoothWallet([FromBody] CreateBoothWalletRequest request)
        {
            try
            {
                var wallet = await _service.CreateBoothWalletAsync(request);
                return Ok(new { success = true, data = wallet });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? boothId,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _service.SearchBoothWalletsAsync(boothId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            [FromQuery] int boothWalletId,
            [FromQuery] decimal totalBalance)
        {
            try
            {
                var result = await _service.UpdateBoothWalletAsync(boothWalletId, totalBalance);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int boothWalletId)
        {
            try
            {
                var result = await _service.DeleteBoothWalletAsync(boothWalletId);
                return Ok(new { success = result, message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
