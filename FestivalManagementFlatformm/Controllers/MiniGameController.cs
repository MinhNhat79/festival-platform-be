using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/minigames")]
    public class MiniGameController : ControllerBase
    {
        private readonly IMiniGameService _service;

        public MiniGameController(IMiniGameService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MinigameCreateRequest request)
        {
            try
            {
                var result = await _service.CreateMinigameAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            int gameId,
            int boothId,
            string gameName,
            string? gameType,
            string? rules,
            int? rewardPoints,
            string? status)
        {
            try
            {
                var result = await _service.UpdateMinigameAsync(gameId, boothId, gameName, gameType, rules, rewardPoints, status);
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
            int? gameId, int? boothId, string? gameName, string? gameType, string? status,
            int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _service.SearchMinigamesAsync(gameId, boothId, gameName, gameType, status, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int gameId)
        {
            try
            {
                await _service.DeleteMinigameAsync(gameId);
                return Ok(new { Success = true, Message = "Xóa thành công" });
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
