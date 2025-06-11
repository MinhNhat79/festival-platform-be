using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IAccountService
    {
        Task<AccountResponse> RegisterStudentAccountBySchoolManager(RegisterRequest request);
        Task<AccountResponse> RegisterAccount(RegisterRequestAll request);
        Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, int? role, int? pageNumber, int? pageSize);
        Task DeleteAccountAsync(int accountId);

        Task<AccountResponse> UpdateAccount(int id, AccountUpdateRequest accountRequest);
    }
}
