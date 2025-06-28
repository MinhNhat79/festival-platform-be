using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

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
        var result = await _service.CreateQuestionAsync(request);
        return Ok(result);
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
        var result = await _service.UpdateQuestionAsync(
            questionId, gameId, questionText, options, correctAnswer, points);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        int? questionId, int? gameId, string? questionText,
        int? pageNumber, int? pageSize)
    {
        var result = await _service.SearchQuestionsAsync(
            questionId, gameId, questionText, pageNumber, pageSize);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(int questionId)
    {
        var result = await _service.DeleteQuestionAsync(questionId);
        return Ok(new { success = result });
    }
}
