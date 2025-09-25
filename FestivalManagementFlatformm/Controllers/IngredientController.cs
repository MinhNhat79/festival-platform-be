using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/ingredients")]
    public class IngredientController : Controller
    {
        private readonly IIngredientService _ingredientService;

        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] IngredientCreateRequest request)
        {
            try
            {
                var result = await _ingredientService.CreateIngredientAsync(request);
                if (result == null)
                    return NotFound(new { success = false, message = "❌ Không tạo được nguyên liệu." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? ingredientId,
            [FromQuery] int? supplierId,
            [FromQuery] string? ingredientName,
            [FromQuery] string? unit,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _ingredientService.SearchIngredientsAsync(
                    ingredientId, supplierId, ingredientName, unit, minPrice, maxPrice, pageNumber, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            [FromQuery] int id,
            [FromQuery] int? supplierId,
            [FromQuery] string? ingredientName,
            [FromQuery] string? description,
            [FromQuery] string? unit,
            [FromQuery] decimal? pricePerUnit)
        {
            try
            {
                var result = await _ingredientService.UpdateIngredientAsync(
                    id, supplierId, ingredientName, description, unit, pricePerUnit);

                if (result == null)
                    return NotFound(new { success = false, message = "❌ Không tìm thấy nguyên liệu để cập nhật." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            try
            {
                await _ingredientService.DeleteIngredientAsync(id);
                

                return Ok(new { success = true, message = "✅ Xóa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
