using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface ISchoolAccountService
    {
        Task<SchoolAccount> CreateSchoolAccountAsync(SchoolAccountCreateRequest request);
        Task<SchoolAccount> UpdateSchoolAccountAsync(int schoolAccountId, string? position, string? department);
        Task<List<SchoolAccount>> SearchSchoolAccountsAsync(int? schoolAccountId, int? schoolId, int? accountId, string? position, string? department, int? pageNumber, int? pageSize);
        Task<bool> DeleteSchoolAccountAsync(int schoolAccountId);
    }
}
