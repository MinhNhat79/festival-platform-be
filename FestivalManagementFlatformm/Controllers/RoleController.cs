using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoleCreateRequest request)
        {
            try
            {
                var result = await _service.CreateRoleAsync(request);
                return Ok(result);
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
        public async Task<IActionResult> Update(int id, [FromQuery] string roleName, [FromQuery] string? permissions)
        {
            try
            {
                var result = await _service.UpdateRoleAsync(id, roleName, permissions);
                return Ok(result);
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
        public async Task<IActionResult> Search(int? roleId, string? roleName, int? pageNumber, int? pageSize)
        {
            try
            {
                var result = await _service.SearchRolesAsync(roleId, roleName, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteRoleAsync(id);
                if (!success)
                    return BadRequest(new { Success = false, Message = "Delete failed" });

                return Ok(new { Success = true, Message = "Deleted successfully" });
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
