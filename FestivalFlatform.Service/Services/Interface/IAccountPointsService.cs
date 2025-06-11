using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IAccountPointsService
    {
        Task<AccountPoints> CreateAccountPointsAsync(int accountId);
        Task<List<AccountPoints>> SearchAccountPointsAsync(int? accountPointsId, int? accountId, int? pageNumber, int? pageSize);
        Task<AccountPoints> UpdateAccountPointsAsync(int accountPointsId, int newPointsBalance);
        Task DeleteAccountPointsAsync(int accountPointsId);

    }
}
