using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/minigames")]
    public class MiniGameController : Controller
    {
        private readonly IMiniGameService _service;

        public MiniGameController(IMiniGameService service)
        {
            _service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MinigameCreateRequest request)
        {
            var result = await _service.CreateMinigameAsync(request);
            return Ok(result);
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
            var result = await _service.UpdateMinigameAsync(
                gameId, boothId, gameName, gameType, rules, rewardPoints, status);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            int? gameId, int? boothId, string? gameName, string? gameType, string? status,
            int? pageNumber, int? pageSize)
        {
            var result = await _service.SearchMinigamesAsync(
                gameId, boothId, gameName, gameType, status, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int gameId)
        {
            var result = await _service.DeleteMinigameAsync(gameId);
            return Ok(result);
        }
    }
}
