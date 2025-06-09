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

    }
}
