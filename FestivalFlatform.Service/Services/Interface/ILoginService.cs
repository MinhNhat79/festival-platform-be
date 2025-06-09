using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Service.DTOs.Response;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface ILoginService
    {
        Task<LoginResponse> AuthenticateAsync(string email, string password);
    }
}
