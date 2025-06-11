using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/schoolaccounts")]
    public class SchoolAccountController : Controller
    {
        private readonly ISchoolAccountService _schoolAccountService;
        public SchoolAccountController(ISchoolAccountService schoolAccountService)
        {
            _schoolAccountService = schoolAccountService;
        }



        [HttpPost("create")]
        public async Task<IActionResult> CreateSchoolAccount([FromBody] SchoolAccountCreateRequest request)
        {
            var result = await _schoolAccountService.CreateSchoolAccountAsync(request);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSchoolAccounts([FromQuery] int? schoolAccountId,[FromQuery] int? schoolId,[FromQuery] int? accountId,[FromQuery] string? position,[FromQuery] string? department,[FromQuery] int? pageNumber,[FromQuery] int? pageSize)
        {
            var result = await _schoolAccountService.SearchSchoolAccountsAsync(
                schoolAccountId, schoolId, accountId, position, department, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSchoolAccount([FromQuery] int schoolAccountId,[FromQuery] string? position,[FromQuery] string? department)
        {
            var result = await _schoolAccountService.UpdateSchoolAccountAsync(schoolAccountId, position, department);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSchoolAccount([FromQuery] int schoolAccountId)
        {
            var result = await _schoolAccountService.DeleteSchoolAccountAsync(schoolAccountId);
            return Ok(new { success = result });
        }
    }
}
