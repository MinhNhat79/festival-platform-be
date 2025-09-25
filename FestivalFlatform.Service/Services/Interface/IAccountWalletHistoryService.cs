using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IAccountWalletHistoryService
    {
        Task<AccountWalletHistory> CreateHistoryAsync(CreateAccountWalletHistoryRequest request);
        Task<AccountWalletHistory> UpdateHistoryAsync(int historyId, string? description);
         Task<List<AccountWalletHistory>> SearchHistoriesAsync(
   int? accountId,
   int? historyId,
   string? type,
   int? pageNumber,
   int? pageSize);
        Task DeleteHistoryAsync(int historyId);
    }
}
