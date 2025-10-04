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
    public class BoothWalletService : IBoothWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public BoothWalletService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BoothWallet> CreateBoothWalletAsync(CreateBoothWalletRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request không được null");

          
            var booth = await _unitOfWork.Repository<Booth>().GetAll()
                .FirstOrDefaultAsync(b => b.BoothId == request.BoothId);

            if (booth == null)
                throw new Exception($"BoothId {request.BoothId} không tồn tại");

            var wallet = new BoothWallet
            {
                BoothId = request.BoothId,
                TotalBalance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<BoothWallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();

            return wallet;
        }


        public async Task<BoothWallet?> GetBoothWalletByIdAsync(int boothWalletId)
        {
            return await _unitOfWork.Repository<BoothWallet>()
                .GetAll()
                .Include(w => w.Booth)
                .FirstOrDefaultAsync(w => w.BoothWalletId == boothWalletId);
        }

        public async Task<IEnumerable<BoothWallet>> SearchBoothWalletsAsync(int? boothId, int? pageNumber, int? pageSize)
        {
            IQueryable<BoothWallet> query = _unitOfWork.Repository<BoothWallet>().GetAll();

            if (boothId.HasValue)
                query = query.Where(w => w.BoothId == boothId.Value);

            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return await query.ToListAsync();
        }

        public async Task<BoothWallet> UpdateBoothWalletAsync(int boothWalletId, decimal totalBalance)
        {
            var wallet = await _unitOfWork.Repository<BoothWallet>()
                .GetAll()
                .FirstOrDefaultAsync(w => w.BoothWalletId == boothWalletId);

            if (wallet == null)
                throw new KeyNotFoundException($"BoothWalletId {boothWalletId} không tồn tại");

            wallet.TotalBalance = totalBalance;
            wallet.UpdatedAt = DateTime.UtcNow;

            
            await _unitOfWork.CommitAsync();

            return wallet;
        }

        public async Task<bool> DeleteBoothWalletAsync(int boothWalletId)
        {
            var wallet = await _unitOfWork.Repository<BoothWallet>()
                .GetAll()
                .FirstOrDefaultAsync(w => w.BoothWalletId == boothWalletId);

            if (wallet == null)
                throw new KeyNotFoundException($"BoothWalletId {boothWalletId} không tồn tại");

            _unitOfWork.Repository<BoothWallet>().Delete(wallet);
            await _unitOfWork.CommitAsync();

            return true;
        }


        public async Task<BoothWallet> UpdateTotalBalanceAsync(int boothId, decimal balance)
        {
            
            var wallet = await _unitOfWork.Repository<BoothWallet>()
                .GetAll()
                .FirstOrDefaultAsync(w => w.BoothId == boothId);

            if (wallet == null)
                throw new KeyNotFoundException($"BoothWallet của BoothId {boothId} không tồn tại");

           
            wallet.TotalBalance += balance;
            wallet.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return wallet;
        }
    }
}
