using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/studentgroups")]
    public class StudentGroupController : ControllerBase
    {
        private readonly IStudentGroupService _studentGroupService;

        public StudentGroupController(IStudentGroupService studentGroupService)
        {
            _studentGroupService = studentGroupService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudentGroup([FromBody] StudentGroupCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Model validation failed",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var created = await _studentGroupService.CreateStudentGroupAsync(request);
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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudentGroup(int groupId,
            [FromQuery] string? className,
            [FromQuery] string? groupName,
            [FromQuery] decimal? groupBudget,
            [FromQuery] string? status)
        {
            try
            {
                var updated = await _studentGroupService.UpdateStudentGroupAsync(groupId, className, groupName, groupBudget, status);
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudentGroups(
            [FromQuery] int? groupId,
            [FromQuery] string? status,
            [FromQuery] int? accountId,
            [FromQuery] int? schoolId,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _studentGroupService.SearchStudentGroupsAsync(groupId, status, accountId, schoolId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteStudentGroup(int groupId)
        {
            try
            {
                 await _studentGroupService.DeleteStudentGroupAsync(groupId);
               
                return Ok(new { Success = true, Message = "Xóa nhóm thành công." });
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
