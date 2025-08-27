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
    public class StudentGroupService : IStudentGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public StudentGroupService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentGroup> CreateStudentGroupAsync(StudentGroupCreateRequest request)
        {
            // ✅ Validate SchoolId tồn tại
            var schoolExists = await _unitOfWork.Repository<School>()
                .AnyAsync(s => s.SchoolId == request.SchoolId);

            if (!schoolExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy trường tương ứng", request.SchoolId.ToString());

            // ✅ Validate AccountId tồn tại
            var accountExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);

            if (!accountExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Account tương ứng", request.AccountId.ToString());

            // ✅ Check trùng GroupName trong cùng 1 trường
            var duplicateGroup = await _unitOfWork.Repository<StudentGroup>()
                .AnyAsync(g => g.SchoolId == request.SchoolId && g.GroupName == request.GroupName);

            if (duplicateGroup)
                throw new CrudException(HttpStatusCode.Conflict, "Tên nhóm đã tồn tại trong trường này", request.GroupName);

            // ✅ Tạo StudentGroup mới
            var group = new StudentGroup
            {
                SchoolId = request.SchoolId,
                AccountId = request.AccountId,
                ClassName = request.ClassName,
                GroupName = request.GroupName,
                GroupBudget = request.GroupBudget ?? 0,
                Status = "active",
                CreationDate = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _unitOfWork.Repository<StudentGroup>().InsertAsync(group);
            await _unitOfWork.CommitAsync();

            return group;
        }

        public async Task<StudentGroup> UpdateStudentGroupAsync(int groupId, string? className, string? groupName, decimal? groupBudget, string? status)
        {
            var group = await _unitOfWork.Repository<StudentGroup>()
                .FindAsync(g => g.GroupId == groupId);

            if (group == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy nhóm sinh viên", groupId.ToString());

            if (!string.IsNullOrWhiteSpace(className))
                group.ClassName = className.Trim();

            if (!string.IsNullOrWhiteSpace(groupName))
                group.GroupName = groupName.Trim();

            if (groupBudget.HasValue)
                group.GroupBudget = groupBudget.Value;

            if (!string.IsNullOrWhiteSpace(status))
                group.Status = status.Trim();

            group.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return group;
        }

        public async Task<List<StudentGroup>> SearchStudentGroupsAsync(
        int? groupId, string? status, int? accountId, int? schoolId,
        int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<StudentGroup>().GetAll()
                .Where(g => !groupId.HasValue || g.GroupId == groupId.Value)
                .Where(g => string.IsNullOrWhiteSpace(status) || g.Status.ToLower().Contains(status.Trim().ToLower()))
                .Where(g => !accountId.HasValue || g.AccountId == accountId.Value)
                .Where(g => !schoolId.HasValue || g.SchoolId == schoolId.Value);

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

          
            return result;
        }

        public async Task DeleteStudentGroupAsync(int groupId)
        {
            var group = await _unitOfWork.Repository<StudentGroup>().GetAll()
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy nhóm để xóa", groupId.ToString());

            _unitOfWork.Repository<StudentGroup>().Delete(group);
            await _unitOfWork.CommitAsync();
        }
    }
}
