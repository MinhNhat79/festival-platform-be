using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/studentgroups")]
    public class StudentGroupController : Controller
    {
        private readonly IStudentGroupService _studentGroupService;

        public StudentGroupController(IStudentGroupService studentservicegroup)
        {
            _studentGroupService = studentservicegroup;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudentGroup([FromBody] StudentGroupCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supplier = await _studentGroupService.CreateStudentGroupAsync(request);

            return Ok(supplier);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudentGroup(int groupId,
            [FromQuery] string? className,
            [FromQuery] string? groupName,
            [FromQuery] decimal? groupBudget,
            [FromQuery] string? status)
        {
            var result = await _studentGroupService.UpdateStudentGroupAsync(groupId, className, groupName, groupBudget, status);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudentGroups(
           [FromQuery] int? groupId,
           [FromQuery] string? status,
           [FromQuery] int? accountId,
           [FromQuery] int? schoolId,
           [FromQuery] int? pageNumber,
           [FromQuery] int? pageSize)
        {
            var result = await _studentGroupService.SearchStudentGroupsAsync(groupId, status, accountId, schoolId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteStudentGroup(int groupId)
        {
            await _studentGroupService.DeleteStudentGroupAsync(groupId);
            return Ok(new { message = "Xóa nhóm thành công." });
        }
    }
}
