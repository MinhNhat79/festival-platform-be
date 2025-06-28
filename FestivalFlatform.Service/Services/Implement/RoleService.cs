using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public RoleService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Role> CreateRoleAsync(RoleCreateRequest request)
        {
            var role = new Role
            {
                RoleName = request.RoleName.Trim(),
                Permissions = request.Permissions?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Role>().InsertAsync(role);
            await _unitOfWork.CommitAsync();
            return role;
        }

        public async Task<Role> UpdateRoleAsync(int roleId, string roleName, string? permissions)
        {
            var role = await _unitOfWork.Repository<Role>().GetAll()
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (role == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Role", roleId.ToString());

            role.RoleName = roleName.Trim();
            role.Permissions = permissions?.Trim();


            await _unitOfWork.CommitAsync();
            return role;
        }
        public async Task<List<Role>> SearchRolesAsync(int? roleId, string? roleName, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Role>().GetAll()
                .Where(r => !roleId.HasValue || r.RoleId == roleId.Value)
                .Where(r => string.IsNullOrWhiteSpace(roleName) || r.RoleName.Contains(roleName.Trim()));

            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);

            return await query.Skip((currentPage - 1) * currentSize).Take(currentSize).ToListAsync();
        }
        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _unitOfWork.Repository<Role>().GetAll()
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (role == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Role", roleId.ToString());

            _unitOfWork.Repository<Role>().Delete(role);
            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
