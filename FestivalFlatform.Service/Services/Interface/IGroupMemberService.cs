using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IGroupMemberService
    {
        Task<GroupMember> CreateGroupMemberAsync(CreateGroupMemberRequest request);
        Task<GroupMember> UpdateGroupMemberAsync(
        int memberId,
        int groupId,
        int accountId,
        string? role);
        Task<List<GroupMember>> SearchGroupMembersAsync(
       int? memberId, int? groupId, int? accountId, string? role,
       int? pageNumber, int? pageSize);
        Task<bool> DeleteGroupMemberAsync(int memberId);
    }
}
