using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
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

        [HttpPost]
        public async Task<IActionResult> CreateImage([FromBody] CreateImageRequest request)
        {
            var result = await _service.CreateImageAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateImage(
       int imageId,
       int? menuItemId,
       string imageUrl,
       string? imageName)
        {
            var result = await _service.UpdateImageAsync(imageId, menuItemId, imageUrl, imageName);
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchImages(
        [FromQuery] int? imageId,
        [FromQuery] int? menuItemId,
        [FromQuery] string? imageUrl,
        [FromQuery] string? imageName,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize)
        {
            var result = await _service.SearchImagesAsync(imageId, menuItemId, imageUrl, imageName, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _service.DeleteImageAsync(id);
            return Ok(new { success = result });
        }
    }
}
