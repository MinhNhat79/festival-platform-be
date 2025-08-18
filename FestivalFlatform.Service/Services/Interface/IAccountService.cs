using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;
using Microsoft.AspNetCore.Http;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IAccountService
    {
        Task<AccountResponse> RegisterStudentAccountBySchoolManager(RegisterRequest request);
        Task<AccountResponse> RegisterAccount(RegisterRequestAll request);
        Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, int? role, int? pageNumber, int? pageSize);
        Task DeleteAccountAsync(int accountId);

        Task<AccountResponse> UpdateAccount(int id, AccountUpdateRequest accountRequest);

        Task<(List<AccountResponse> successAccounts, List<string> errors)> ImportAccountsFromExcelAsync(int schoolId, Stream excelStream);
        Task<bool> UpdatePasswordAsync(int accountId, string oldPassword, string newPassword);
    }
}
