using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IStudentGroupService
    {
        Task<StudentGroup> CreateStudentGroupAsync(StudentGroupCreateRequest request);
        Task<StudentGroup> UpdateStudentGroupAsync(int groupId, string? className, string? groupName, decimal? groupBudget, string? status);
        Task<List<StudentGroup>> SearchStudentGroupsAsync(
        int? groupId, string? status, int? accountId, int? schoolId,
        int? pageNumber, int? pageSize);
        Task DeleteStudentGroupAsync(int groupId);
    }
}
