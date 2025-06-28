using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class PointsTransactionService : IPointsTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public PointsTransactionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PointsTransaction> CreatePointsTransactionAsync(PointsTransactionCreateRequest request)
        {
            if (request.PointsAmount <= 0)
                throw new ArgumentException("PointsAmount must be greater than 0");

            if (request.TransactionType != "earned" && request.TransactionType != "spent")
                throw new ArgumentException("TransactionType must be 'earned' or 'spent'");

            var exists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);
            if (!exists)
                throw new ArgumentException("Account not found");

            var tx = new PointsTransaction
            {
                AccountId = request.AccountId,
                BoothId = request.BoothId,
                GameId = request.GameId,
                PointsAmount = request.PointsAmount,
                TransactionType = request.TransactionType.Trim().ToLower(),
                TransactionDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PointsTransaction>().InsertAsync(tx);
            await _unitOfWork.CommitAsync();

            return tx;
        }

        public async Task<PointsTransaction> UpdatePointsTransactionAsync(
        int pointsTxId,
        int pointsAmount,
        string transactionType)
        {
            var tx = await _unitOfWork.Repository<PointsTransaction>().GetAll()
                .FirstOrDefaultAsync(p => p.PointsTxId == pointsTxId);

            if (tx == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy transaction", pointsTxId.ToString());

            if (pointsAmount <= 0)
                throw new ArgumentException("PointsAmount must be greater than 0");

            if (transactionType != "earned" && transactionType != "spent")
                throw new ArgumentException("TransactionType must be 'earned' or 'spent'");

            tx.PointsAmount = pointsAmount;
            tx.TransactionType = transactionType.Trim().ToLower();
            tx.TransactionDate = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return tx;
        }
        public async Task<List<PointsTransaction>> SearchPointsTransactionsAsync(
        int? accountId, string? transactionType, int? gameId, int? boothId,
        int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<PointsTransaction>().GetAll()
                .Where(p => !accountId.HasValue || p.AccountId == accountId.Value)
                .Where(p => string.IsNullOrWhiteSpace(transactionType) || p.TransactionType == transactionType.Trim().ToLower())
                .Where(p => !gameId.HasValue || p.GameId == gameId)
                .Where(p => !boothId.HasValue || p.BoothId == boothId);

            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);
            query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            return await query.ToListAsync();
        }
        public async Task<bool> DeletePointsTransactionAsync(int pointsTxId)
        {
            var tx = await _unitOfWork.Repository<PointsTransaction>().FindAsync(p => p.PointsTxId == pointsTxId);
            if (tx == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy transaction", pointsTxId.ToString());

            _unitOfWork.Repository<PointsTransaction>().Delete(tx);
            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
