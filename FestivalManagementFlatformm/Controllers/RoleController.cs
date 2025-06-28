using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : Controller
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoleCreateRequest request)
        {
            var result = await _service.CreateRoleAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, [FromQuery] string roleName, [FromQuery] string? permissions)
        {
            var result = await _service.UpdateRoleAsync(id, roleName, permissions);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? roleId, string? roleName, int? pageNumber, int? pageSize)
        {
            var result = await _service.SearchRolesAsync(roleId, roleName, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteRoleAsync(id);
            return Ok(result);
        }
    }
}
