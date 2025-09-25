using FestivalFlatform.Service.DTOs.Request;
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
            try
            {
                var result = await _service.CreateGroupMemberAsync(request);
                if (result == null)
                    return NotFound(new { success = false, message = "❌ Không tìm thấy nhóm hoặc tài khoản." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroupMember(
            [FromQuery] int memberId,
            [FromQuery] int groupId,
            [FromQuery] int accountId,
            [FromQuery] string? role)
        {
            try
            {
                var result = await _service.UpdateGroupMemberAsync(memberId, groupId, accountId, role);
                if (result == null)
                    return NotFound(new { success = false, message = "❌ Không tìm thấy thành viên nhóm." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
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
            try
            {
                var result = await _service.SearchGroupMembersAsync(memberId, groupId, accountId, role, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteGroupMember([FromQuery] int memberId)
        {
            try
            {
                var deleted = await _service.DeleteGroupMemberAsync(memberId);
                if (!deleted)
                    return NotFound(new { success = false, message = "❌ Không tìm thấy thành viên nhóm để xóa." });

                return Ok(new { success = true, message = "✅ Xóa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
