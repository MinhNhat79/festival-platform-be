using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class AccountPointsService : IAccountPointsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public AccountPointsService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<AccountPoints> CreateAccountPointsAsync(int accountId)
        {
          
            var account = await _unitOfWork.Repository<Account>()
                .FindAsync(a => a.AccountId == accountId);

            if (account == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy tài khoản với ID: {accountId}");
            }

            var existingPoints = await _unitOfWork.Repository<AccountPoints>()
                .FindAsync(ap => ap.AccountId == accountId);

            if (existingPoints != null)
            {
                throw new InvalidOperationException($"AccountPoints đã tồn tại cho tài khoản ID: {accountId}");
            }

            var accountPoints = new AccountPoints
            {
                AccountId = accountId,
                PointsBalance = 0,
                LastUpdated = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<AccountPoints>().InsertAsync(accountPoints);
            await _unitOfWork.CommitAsync();

            return accountPoints;
        }

        public async Task<List<AccountPoints>> SearchAccountPointsAsync( int? accountPointsId, int? accountId, int? pageNumber,int? pageSize)
        {
            var query = _unitOfWork.Repository<AccountPoints>()
                .GetAll()
                .Where(ap =>
                    (!accountPointsId.HasValue || ap.AccountPointsId == accountPointsId) &&
                    (!accountId.HasValue || ap.AccountId == accountId) 

                );

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            var result = await query
                .Skip((currentPage - 1) * currentSize)
                .Take(currentSize)
                .ToListAsync();

            return result;
        }

        public async Task<AccountPoints> UpdateAccountPointsAsync(int accountPointsId, int newPointsBalance)
        {
          
            var accountPoints = await _unitOfWork.Repository<AccountPoints>()
                .FindAsync(ap => ap.AccountPointsId == accountPointsId);

            if (accountPoints == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy AccountPoints với ID: {accountPointsId}");
            }

          
            accountPoints.PointsBalance = newPointsBalance;
            accountPoints.LastUpdated = DateTime.UtcNow;

            await _unitOfWork.Repository<AccountPoints>().Update(accountPoints, accountPointsId);
            await _unitOfWork.CommitAsync();

            return accountPoints;
        }

        public async Task DeleteAccountPointsAsync(int accountPointsId)
        {
            var accountPoints = await _unitOfWork.Repository<AccountPoints>()
                .FindAsync(ap => ap.AccountPointsId == accountPointsId);

            if (accountPoints == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy AccountPoints với ID: {accountPointsId}");
            }

            _unitOfWork.Repository<AccountPoints>().Delete(accountPoints);
            await _unitOfWork.CommitAsync();
        }

    }
}
