using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/schools")]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSchool([FromBody] SchoolCreateRequest request)
        {
            try
            {
                var created = await _schoolService.CreateSchoolAsync(request);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSchools(
            [FromQuery] int? id,
            [FromQuery] int? accountId,
            [FromQuery] string? name,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _schoolService.SearchSchoolsAsync(id, accountId, name, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSchool(int id, string? contactInfo, string? logoUrl, string? description)
        {
            try
            {
                var updated = await _schoolService.UpdateSchoolAsync(id, contactInfo, logoUrl, description);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            try
            {
                await _schoolService.DeleteSchoolAsync(id);
               
                   

                return Ok(new { Success = true, Message = "Xóa school thành công" });
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
