using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class WalletService :IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public WalletService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Wallet> CreateWalletAsync(CreateWalletRequest request)
        {

            var userExists = await _unitOfWork.Repository<Account>()
            .AnyAsync(x => x.AccountId == request.AccountId);
            if (!userExists)
                throw new ArgumentException("Không tìm thấy tài khoản với accountId đã cung cấp");

            var wallet = new Wallet
            {
                AccountId = request.AccountId,
                Balance = request.Balance,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();

            return wallet;
        }

        public async Task<Wallet> UpdateWalletAsync(int id, decimal balance)
        {
            var wallet = await _unitOfWork.Repository<Wallet>().GetAll().FirstOrDefaultAsync(w => w.WalletId ==id)
                ?? throw new Exception("Wallet not found");

            wallet.Balance = balance;
            await _unitOfWork.CommitAsync();

            return wallet;
        }

        public async Task<List<Wallet>> SearchWalletsAsync(
        int? accountid,
        int? pageNumber,
        int? pageSize)
        {
            var query = _unitOfWork.Repository<Wallet>().GetAll()
                .Where(w => !accountid.HasValue || w.AccountId == accountid);

            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);

            query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteWalletAsync(int id)
        {
            var wallet = await _unitOfWork.Repository<Wallet>().GetAll().FirstOrDefaultAsync(w => w.WalletId == id)
                ?? throw new Exception("Wallet not found");

            _unitOfWork.Repository<Wallet>().Delete(wallet);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
