using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IWalletService
    {
        Task<Wallet> CreateWalletAsync(CreateWalletRequest request);
        Task<Wallet> UpdateWalletAsync(int id, decimal balance);
        Task<List<Wallet>> SearchWalletsAsync(
       int? accountid,
       int? pageNumber,
       int? pageSize);
        Task<bool> DeleteWalletAsync(int id);
    }
}
