using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/menuitemingredients")]
    public class MenuItemIngredientController : Controller
    {
        private readonly IMenuItemIngredientService _service;

        public MenuItemIngredientController(IMenuItemIngredientService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemIngredientRequest request)
        {
            var result = await _service.CreateMenuItemIngredientAsync(request);
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(
               [FromQuery] int? itemIngredientId,
               [FromQuery] int? itemId,
               [FromQuery] int? ingredientId,
               [FromQuery] string? unit,
               [FromQuery] int? pageNumber,
               [FromQuery] int? pageSize)
        {
            var result = await _service.SearchMenuItemIngredientsAsync(itemIngredientId, itemId, ingredientId, unit, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
         int itemIngredientId,
        int itemId,
        int ingredientId,
        decimal quantity,
        string unit)
        {
            var result = await _service.UpdateMenuItemIngredientAsync(itemIngredientId, itemId, ingredientId, quantity, unit);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteMenuItemIngredientAsync(id);
            return success ? NoContent() : BadRequest();
        }
    }
}
