using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IRoleService
    {
        Task<Role> CreateRoleAsync(RoleCreateRequest request);
        Task<Role> UpdateRoleAsync(int roleId, string roleName, string? permissions);
        Task<List<Role>> SearchRolesAsync(int? roleId, string? roleName, int? pageNumber, int? pageSize);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}
