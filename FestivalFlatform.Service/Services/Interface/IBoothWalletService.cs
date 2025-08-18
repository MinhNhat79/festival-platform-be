using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IBoothWalletService
    {
        Task<BoothWallet?> GetBoothWalletByIdAsync(int boothWalletId);
        Task<IEnumerable<BoothWallet>> SearchBoothWalletsAsync(int? boothId, int? pageNumber, int? pageSize);
        Task<BoothWallet> UpdateBoothWalletAsync(int boothWalletId, decimal totalBalance);
        Task<bool> DeleteBoothWalletAsync(int boothWalletId);
        Task<BoothWallet> CreateBoothWalletAsync(CreateBoothWalletRequest request);
    }
}
