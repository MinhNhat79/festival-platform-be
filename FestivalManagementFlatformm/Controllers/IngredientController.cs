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
            var result = await _ingredientService.CreateIngredientAsync(request);
            return Ok(result);
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
            var result = await _ingredientService.SearchIngredientsAsync(
                ingredientId, supplierId, ingredientName, unit, minPrice, maxPrice, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id,
           [FromQuery] int? supplierId,
           [FromQuery] string? ingredientName,
           [FromQuery] string? description,
           [FromQuery] string? unit,
           [FromQuery] decimal? pricePerUnit)
        {
            var result = await _ingredientService.UpdateIngredientAsync(id, supplierId, ingredientName, description, unit, pricePerUnit);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ingredientService.DeleteIngredientAsync(id);
            return NoContent();
        }
    }
}
