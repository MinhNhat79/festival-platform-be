using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var supplier = await _supplierService.CreateSupplierAsync(request);
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStatusAndRating(int supplierId, string? status, decimal? rating)
        {
            try
            {
                var result = await _supplierService.UpdateSupplierStatusAndRatingAsync(supplierId, status, rating);

                if (!result)
                    return BadRequest(new { Success = false, Message = "Update failed." });

                return Ok(new { Success = true, Message = "Update successful." });
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchSuppliers(
            [FromQuery] int? supplierId,
            [FromQuery] int? accountId,
            [FromQuery] string? companyName,
            [FromQuery] string? status,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _supplierService.GetSuppliers(supplierId, accountId, companyName, status, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSupplier(int supplierId)
        {
            try
            {
                await _supplierService.DeleteSupplier(supplierId);
                return Ok(new { Success = true, Message = "Deleted successfully." });
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
