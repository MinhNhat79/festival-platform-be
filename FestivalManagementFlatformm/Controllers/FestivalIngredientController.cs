using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/festivalingredients")]
    public class FestivalIngredientController : Controller
    {
        private readonly IFestivalIngredientService _festivalIngredientService;

        public FestivalIngredientController(IFestivalIngredientService festivalIngredientService)
        {
            _festivalIngredientService = festivalIngredientService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FestivalIngredientCreateRequest request)
        {
            var result = await _festivalIngredientService.CreateFestivalIngredientAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, int? festivalId, int? ingredientId, int? quantityAvailable, decimal? specialPrice, string? status)
        {
            var result = await _festivalIngredientService.UpdateFestivalIngredientAsync(id, festivalId, ingredientId, quantityAvailable, specialPrice, status);
            return Ok(result);
        }

        [HttpPut("update/rejected")]
        public async Task<IActionResult> Rejected(int id, string? rejectReason)
        {
            await _festivalIngredientService.UpdateFestivaIngredientStatusToRejectAsync(id, rejectReason);
            return Ok("Cập nhật trạng thái 'reject' thành công");
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(int? festivalIngredientId, int? festivalId, int? ingredientId, string? status, int? pageNumber, int? pageSize)
        {
            var result = await _festivalIngredientService.SearchFestivalIngredientsAsync(festivalIngredientId, festivalId, ingredientId, status, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _festivalIngredientService.DeleteFestivalIngredientAsync(id);
            return Ok();
        }
    }
}
