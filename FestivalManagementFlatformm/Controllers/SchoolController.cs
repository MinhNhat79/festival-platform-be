using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/schools")]
    public class SchoolController : Controller
    {
        private readonly ISchoolService _schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSchool([FromBody] SchoolCreateRequest request)
        {
            var created = await _schoolService.CreateSchoolAsync(request);
            return Ok(created);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchSchools([FromQuery] int? id, int? accountId, [FromQuery] string? name, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var result = await _schoolService.SearchSchoolsAsync(id, accountId, name, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateSchool(int id, string? contactInfo, string? logoUrl, string? description)
        {
            var updated = await _schoolService.UpdateSchoolAsync(id, contactInfo, logoUrl, description);
            return Ok(updated);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            await _schoolService.DeleteSchoolAsync(id);
            return NoContent();
        }
    }
}
