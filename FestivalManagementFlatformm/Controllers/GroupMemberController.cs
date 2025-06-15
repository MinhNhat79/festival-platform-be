using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using System.Net;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/groupmembers")]
    public class GroupMemberController : Controller
    {
        private readonly IGroupMemberService _service;

        public GroupMemberController(IGroupMemberService service)
        {
            _service = service;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateGroupMember([FromBody] CreateGroupMemberRequest request)
        {
            var result = await _service.CreateGroupMemberAsync(request);
            return StatusCode((int)HttpStatusCode.Created, result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroupMember(
       [FromQuery] int memberId,
       [FromQuery] int groupId,
       [FromQuery] int accountId,
       [FromQuery] string? role)
        {
            var result = await _service.UpdateGroupMemberAsync(memberId, groupId, accountId, role);
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchGroupMembers(
      [FromQuery] int? memberId,
      [FromQuery] int? groupId,
      [FromQuery] int? accountId,
      [FromQuery] string? role,
      [FromQuery] int? pageNumber,
      [FromQuery] int? pageSize)
        {
            var result = await _service.SearchGroupMembersAsync(memberId, groupId, accountId, role, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteGroupMember([FromQuery] int memberId)
        {
            var result = await _service.DeleteGroupMemberAsync(memberId);
            return Ok(new { success = result });
        }
    }
}
