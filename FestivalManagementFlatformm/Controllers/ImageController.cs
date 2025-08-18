using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : Controller
    {
        private readonly IImageService _service;

        public ImageController(IImageService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateImage([FromBody] CreateImageRequest request)
        {
            try
            {
                var result = await _service.CreateImageAsync(request);
                if (result == null)
                    return NotFound(new { success = false, message = "❌ Không tạo được ảnh." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateImage(
            [FromQuery] int imageId,
            [FromQuery] int? menuItemId,
            [FromQuery] string imageUrl,
            [FromQuery] string? imageName,
            [FromQuery] int? festivalId,
            [FromQuery] int? boothId
        )
        {
            try
            {
                var result = await _service.UpdateImageAsync(imageId, imageUrl, imageName, festivalId, boothId, menuItemId);
                if (result == null)
                    return NotFound(new { success = false, message = "❌ Không tìm thấy ảnh để cập nhật." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchImages(
            [FromQuery] int? imageId,
            [FromQuery] int? menuItemId,
            [FromQuery] int? boothId,
            [FromQuery] int? festivalId,
            [FromQuery] string? imageUrl,
            [FromQuery] string? imageName,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _service.SearchImagesAsync(
                    imageId,
                    menuItemId,
                    boothId,
                    festivalId,
                    imageUrl,
                    imageName,
                    pageNumber,
                    pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage([FromQuery] int id)
        {
            try
            {
                var deleted = await _service.DeleteImageAsync(id);
                if (!deleted)
                    return NotFound(new { success = false, message = "❌ Không tìm thấy ảnh để xóa." });

                return Ok(new { success = true, message = "✅ Xóa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("add-to-entity")]
        public async Task<IActionResult> AddImageToEntity([FromBody] AddImageToEntityRequest request)
        {
            try
            {
                var image = await _service.AddImageToEntityAsync(request);
                if (image == null)
                    return NotFound(new { success = false, message = "❌ Không thêm được ảnh vào entity." });

                return Ok(image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}
