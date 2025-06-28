using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IPointsTransactionService
    {
        Task<PointsTransaction> CreatePointsTransactionAsync(PointsTransactionCreateRequest request);
        Task<PointsTransaction> UpdatePointsTransactionAsync(
        int pointsTxId,
        int pointsAmount,
        string transactionType);
        Task<List<PointsTransaction>> SearchPointsTransactionsAsync(
        int? accountId, string? transactionType, int? gameId, int? boothId,
        int? pageNumber, int? pageSize);
        Task<bool> DeletePointsTransactionAsync(int pointsTxId);
    }
}
