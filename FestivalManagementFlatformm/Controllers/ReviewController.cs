using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequest request)
        {
            try
            {
                var review = await _reviewService.CreateReviewAsync(request);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo review", detail = ex.Message });
            }
        }

        // UPDATE
        [HttpPut("update")]
        public async Task<IActionResult> UpdateReview(
            int reviewId,
            [FromQuery] int? rating,
            [FromQuery] string? comment)
        {
            try
            {
                var review = await _reviewService.UpdateReviewAsync(reviewId, rating, comment);
                return Ok(review);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật review", detail = ex.Message });
            }
        }

        // SEARCH
        [HttpGet("search")]
        public async Task<IActionResult> SearchReviews(
            [FromQuery] int? reviewId,
            [FromQuery] int? festivalId,
            [FromQuery] int? accountId,
            [FromQuery] int? rating,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var reviews = await _reviewService.SearchReviewsAsync(reviewId, festivalId, accountId, rating, pageNumber, pageSize);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm review", detail = ex.Message });
            }
        }

        // DELETE
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(reviewId);
                return Ok(new { success = true, message = "Xóa review thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa review", detail = ex.Message });
            }
        }
    }
}


