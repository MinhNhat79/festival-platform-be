using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/schoolaccountrelations")]
    public class SchoolAccountRelationController : Controller
    {
        private readonly ISchoolAccountRelationService _service;

        public SchoolAccountRelationController(ISchoolAccountRelationService service)
        {
            _service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateSchoolAccountRelationRequest request)
        {
            var result = await _service.CreateRelationAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, string relationType)
        {
            var result = await _service.UpdateRelationAsync(id, relationType);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? schoolId, int? accountId, string? relationType)
        {
            var result = await _service.SearchRelationsAsync(schoolId, accountId, relationType);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteRelationAsync(id);
            return Ok(new { success = result });
        }
    }
}
