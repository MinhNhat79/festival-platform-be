using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : Controller
    {

        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CommentCreateRequest request)
        {
            try
            {
                var comment = await _commentService.CreateCommentAsync(request);
                return Ok(new { success = true, data = comment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

       
        [HttpPut("update")]
        public async Task<IActionResult> Update(int commentId, string? content)
        {
            try
            {
                var comment = await _commentService.UpdateCommentAsync(commentId, content);
                if (comment == null)
                    return NotFound(new { success = false, message = "Không tìm thấy comment" });

                return Ok(new { success = true, data = comment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

 
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? commentId,
            [FromQuery] int? reviewId,
            [FromQuery] int? accountId,
            [FromQuery] int? parentCommentId,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var comments = await _commentService.SearchCommentsAsync(
                    commentId, reviewId, accountId, parentCommentId, pageNumber, pageSize);

                return Ok(new { success = true, data = comments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

      
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int commentId)
        {
            try
            {
                await _commentService.DeleteCommentAsync(commentId);
                return Ok(new { success = true, message = "Xóa comment thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
