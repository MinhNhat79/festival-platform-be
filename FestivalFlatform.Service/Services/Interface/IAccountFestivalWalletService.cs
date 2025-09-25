using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IAccountFestivalWalletService
    {
        Task<AccountFestivalWallet> CreateAccountFestivalWalletAsync(AccountFestivalWalletCreateRequest request);
        Task<List<AccountFestivalWallet>> SearchAccountFestivalWalletsAsync(
           int? accountFestivalWalletId, int? accountId, int? festivalId, int? pageNumber, int? pageSize);
        Task<AccountFestivalWallet> UpdateAccountFestivalWalletAsync(int id, decimal newBalance, string? newName);
        Task DeleteAccountFestivalWalletAsync(int id);
        Task TransferToAccountFestivalWalletAsync(WalletTransferRequest request);
        Task TransferToMainWalletAsync(WalletTransferRequest request);
    }
}
