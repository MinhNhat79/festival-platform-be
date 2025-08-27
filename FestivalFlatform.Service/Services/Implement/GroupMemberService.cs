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
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public GroupMemberService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<GroupMember> CreateGroupMemberAsync(CreateGroupMemberRequest request)
        {
            // Kiểm tra tồn tại StudentGroup
            var groupExists = await _unitOfWork.Repository<StudentGroup>()
                .AnyAsync(g => g.GroupId == request.GroupId);
            if (!groupExists)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy StudentGroup", request.GroupId.ToString());
            }

            // Kiểm tra tồn tại Account
            var accountExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);
            if (!accountExists)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy Account", request.AccountId.ToString());
            }

            // ✅ Kiểm tra account đã trong group chưa
            var alreadyInGroup = await _unitOfWork.Repository<GroupMember>()
                .AnyAsync(gm => gm.GroupId == request.GroupId && gm.AccountId == request.AccountId);

            if (alreadyInGroup)
            {
                throw new CrudException(HttpStatusCode.BadRequest,
                    "Tài khoản này đã là thành viên của nhóm",
                    $"{request.GroupId}-{request.AccountId}");
            }

            var newMember = new GroupMember
            {
                GroupId = request.GroupId,
                AccountId = request.AccountId,
                Role = request.Role?.Trim(),
                JoinDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<GroupMember>().InsertAsync(newMember);
            await _unitOfWork.CommitAsync();

            return newMember;
        }


        public async Task<GroupMember> UpdateGroupMemberAsync(
        int memberId,
        int groupId,
        int accountId,
        string? role)
        {
            var member = await _unitOfWork.Repository<GroupMember>().GetAll()
                .FirstOrDefaultAsync(x => x.MemberId == memberId);

            if (member == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy GroupMember", memberId.ToString());

            // Check group tồn tại
            var groupExists = await _unitOfWork.Repository<StudentGroup>()
                .AnyAsync(g => g.GroupId == groupId);
            if (!groupExists)
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy StudentGroup", groupId.ToString());

            // Check account tồn tại
            var accountExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == accountId);
            if (!accountExists)
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy Account", accountId.ToString());

            member.GroupId = groupId;
            member.AccountId = accountId;
            member.Role = role?.Trim();

            await _unitOfWork.CommitAsync();
            return member;
        }
        public async Task<List<GroupMember>> SearchGroupMembersAsync(
        int? memberId, int? groupId, int? accountId, string? role,
        int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<GroupMember>().GetAll()
                .Where(m => !memberId.HasValue || m.MemberId == memberId.Value)
                .Where(m => !groupId.HasValue || m.GroupId == groupId.Value)
                .Where(m => !accountId.HasValue || m.AccountId == accountId.Value)
                .Where(m => string.IsNullOrWhiteSpace(role) || m.Role != null && m.Role.Contains(role.Trim()));

            //int currentPage = pageNumber.GetValueOrDefault(1);
            //int currentSize = pageSize.GetValueOrDefault(10);

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

        
            return result;
        }

        public async Task<bool> DeleteGroupMemberAsync(int memberId)
        {
            var member = await _unitOfWork.Repository<GroupMember>().GetAll()
                .FirstOrDefaultAsync(x => x.MemberId == memberId);

            if (member == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy GroupMember", memberId.ToString());

            _unitOfWork.Repository<GroupMember>().Delete(member);
            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
