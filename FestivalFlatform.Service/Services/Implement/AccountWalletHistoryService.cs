using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class AccountWalletHistoryService : IAccountWalletHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public AccountWalletHistoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccountWalletHistory> CreateHistoryAsync(CreateAccountWalletHistoryRequest request)
        {
            var account = await _unitOfWork.Repository<Account>()
                .FindAsync(a => a.AccountId == request.AccountId);

            if (account == null)
                throw new KeyNotFoundException($"Không tìm thấy tài khoản ID: {request.AccountId}");

            var history = new AccountWalletHistory
            {
                AccountId = request.AccountId,
                Description = request.Description?.Trim() ?? string.Empty,
                Amount = request.Amount,
                Type = request.Type?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<AccountWalletHistory>().InsertAsync(history);
            await _unitOfWork.CommitAsync();

            return history;
        }
        public async Task<AccountWalletHistory> UpdateHistoryAsync(int historyId, string? description)
        {
            var history = await _unitOfWork.Repository<AccountWalletHistory>()
                .FindAsync(h => h.HistoryId == historyId);

            if (history == null)
                throw new KeyNotFoundException($"Không tìm thấy lịch sử với ID: {historyId}");

            history.Description = description?.Trim() ?? string.Empty;
            history.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return history;
        }
        public async Task<List<AccountWalletHistory>> SearchHistoriesAsync(
    int? accountId,
    int? historyId,
    string? type,
    int? pageNumber,
    int? pageSize)
        {
            var query = _unitOfWork.Repository<AccountWalletHistory>()
                .GetAll()
                .Where(h =>
                    (!historyId.HasValue || h.HistoryId == historyId) &&
                    (!accountId.HasValue || h.AccountId == accountId) &&
                    (string.IsNullOrEmpty(type) || h.Type == type)
                );

           
            // int currentPage = pageNumber ?? 1;
            // int currentSize = pageSize ?? 10;
            // query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            return await query.ToListAsync();
        }

        public async Task DeleteHistoryAsync(int historyId)
        {
            var history = await _unitOfWork.Repository<AccountWalletHistory>()
                .FindAsync(h => h.HistoryId == historyId);

            if (history == null)
                throw new KeyNotFoundException($"Không tìm thấy lịch sử với ID: {historyId}");

            _unitOfWork.Repository<AccountWalletHistory>().Delete(history);
            await _unitOfWork.CommitAsync();
        }
    }
}

