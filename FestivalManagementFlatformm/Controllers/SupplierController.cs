using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    public class SupplierController : Controller
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
                return BadRequest(ModelState);

            var supplier = await _supplierService.CreateSupplierAsync(request);

            return Ok(supplier);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStatusAndRating(int supplierId, string? status, decimal? rating )
        {
            var result = await _supplierService.UpdateSupplierStatusAndRatingAsync(supplierId, status, rating);
            if (!result)
                return NotFound("Supplier not found.");

            return Ok("Update successful.");
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchSuppliers([FromQuery] int? supplierId,[FromQuery] int? accountId,[FromQuery] string? companyName,[FromQuery] string? status, [FromQuery] int? pageNumber,[FromQuery] int? pageSize)
        {
            var result = await _supplierService.GetSuppliers(supplierId, accountId, companyName, status, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSupplier(int supplierId)
        {
            await _supplierService.DeleteSupplier(supplierId);
            return NoContent();
        }
    }
}
