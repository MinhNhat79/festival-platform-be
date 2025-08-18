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
            try
            {
                var result = await _festivalIngredientService.CreateFestivalIngredientAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, int? festivalId, int? ingredientId, int? quantityAvailable, decimal? specialPrice, string? status)
        {
            try
            {
                var result = await _festivalIngredientService.UpdateFestivalIngredientAsync(id, festivalId, ingredientId, quantityAvailable, specialPrice, status);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [HttpPut("update/rejected")]
        public async Task<IActionResult> Rejected(int id, string? rejectReason)
        {
            try
            {
                await _festivalIngredientService.UpdateFestivaIngredientStatusToRejectAsync(id, rejectReason);
                return Ok(new { success = true, message = "Cập nhật trạng thái 'reject' thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? festivalIngredientId, int? festivalId, int? ingredientId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _festivalIngredientService.SearchFestivalIngredientsAsync(festivalIngredientId, festivalId, ingredientId, status, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tìm kiếm", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _festivalIngredientService.DeleteFestivalIngredientAsync(id);
                return Ok(new { success = true });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
    }
}
