using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FestivalManagementFlatformm.Controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _service;

        public QuestionController(IQuestionService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] QuestionCreateRequest request)
        {
            try
            {
                var result = await _service.CreateQuestionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            int questionId,
            int gameId,
            string questionText,
            string options,
            string correctAnswer,
            int points)
        {
            try
            {
                var result = await _service.UpdateQuestionAsync(questionId, gameId, questionText, options, correctAnswer, points);
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
            int? questionId, int? gameId, string? questionText,
            int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _service.SearchQuestionsAsync(questionId, gameId, questionText, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int questionId)
        {
            try
            {
                var success = await _service.DeleteQuestionAsync(questionId);
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
