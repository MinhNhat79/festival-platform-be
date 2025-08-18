using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
            try
            {
                var result = await _service.CreatePointsTransactionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            [FromQuery] int pointsTxId,
            [FromQuery] int pointsAmount,
            [FromQuery] string transactionType)
        {
            try
            {
                var result = await _service.UpdatePointsTransactionAsync(pointsTxId, pointsAmount, transactionType);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
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
            try
            {
                var result = await _service.SearchPointsTransactionsAsync(accountId, transactionType, gameId, boothId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeletePointsTransactionAsync(id);
                if (!success)
                    return BadRequest(new { Success = false, Message = "Delete failed" });

                return Ok(new { Success = true, Message = "Deleted successfully" });
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
    }
}
