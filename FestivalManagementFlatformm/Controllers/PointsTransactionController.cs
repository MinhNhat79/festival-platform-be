using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;

using Microsoft.AspNetCore.Mvc;

namespace FestivalFlatform.API.Controllers
{
    [ApiController]
    [Route("api/pointstransactions")]
    public class PointsTransactionController : ControllerBase
    {
        private readonly IPointsTransactionService _service;

        public PointsTransactionController(IPointsTransactionService service)
        {
            _service = service;
        }

     
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PointsTransactionCreateRequest request)
        {
            var result = await _service.CreatePointsTransactionAsync(request);
            return Ok(result);
        }

       
        [HttpPut("update")]
        public async Task<IActionResult> Update(
            [FromQuery] int pointsTxId,
            [FromQuery] int pointsAmount,
            [FromQuery] string transactionType)
        {
            var result = await _service.UpdatePointsTransactionAsync(pointsTxId, pointsAmount, transactionType);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? accountId,
            [FromQuery] string? transactionType,
            [FromQuery] int? gameId,
            [FromQuery] int? boothId,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var result = await _service.SearchPointsTransactionsAsync(accountId, transactionType, gameId, boothId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeletePointsTransactionAsync(id);
            return Ok(new { success });
        }
    }
}
