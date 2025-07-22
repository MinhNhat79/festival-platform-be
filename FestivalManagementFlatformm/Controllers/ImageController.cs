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

        [HttpPost("create")]
        public async Task<IActionResult> CreateImage([FromBody] CreateImageRequest request)
        {
            var result = await _service.CreateImageAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateImage(
       int imageId,
       int? menuItemId,
       string imageUrl,
       string? imageName,
       int? festivalId,
       int? boothId
    )
        {
            var result = await _service.UpdateImageAsync(imageId, imageUrl, imageName, festivalId,boothId, menuItemId);
            return Ok(result);
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
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _service.DeleteImageAsync(id);
            return Ok(new { success = result });
        }

        [HttpPost("add-to-entity")]
        public async Task<IActionResult> AddImageToEntity([FromBody] AddImageToEntityRequest request)
        {
            var image = await _service.AddImageToEntityAsync(request);
            return Ok(image);
        }
    }
}
