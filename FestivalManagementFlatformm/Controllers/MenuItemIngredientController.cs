using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/menuitemingredients")]
    public class MenuItemIngredientController : ControllerBase
    {
        private readonly IMenuItemIngredientService _service;

        public MenuItemIngredientController(IMenuItemIngredientService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemIngredientRequest request)
        {
            try
            {
                var result = await _service.CreateMenuItemIngredientAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
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
            try
            {
                var result = await _service.SearchMenuItemIngredientsAsync(itemIngredientId, itemId, ingredientId, unit, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(
            int itemIngredientId,
            int itemId,
            int ingredientId,
            decimal quantity,
            string unit)
        {
            try
            {
                var result = await _service.UpdateMenuItemIngredientAsync(itemIngredientId, itemId, ingredientId, quantity, unit);
                return Ok(result);
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

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteMenuItemIngredientAsync(id);
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
