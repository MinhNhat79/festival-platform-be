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
    public class AccountFestivalWalletService : IAccountFestivalWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public AccountFestivalWalletService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<AccountFestivalWallet> CreateAccountFestivalWalletAsync(AccountFestivalWalletCreateRequest request)
        {
            // Kiểm tra Festival tồn tại
            var festival = await _unitOfWork.Repository<Festival>()
                .FindAsync(f => f.FestivalId == request.FestivalId);

            if (festival == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Festival", request.FestivalId.ToString());
            }

            // Kiểm tra Account tồn tại
            var account = await _unitOfWork.Repository<Account>()
                .FindAsync(a => a.AccountId == request.AccountId);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", request.AccountId.ToString());
            }

            // Kiểm tra ví đã tồn tại chưa
            var existed = await _unitOfWork.Repository<AccountFestivalWallet>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.AccountId == request.AccountId && x.FestivalId == request.FestivalId);

            if (existed != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Ví đã tồn tại cho tài khoản này trong lễ hội", "");
            }

            var wallet = new AccountFestivalWallet
            {
                AccountId = request.AccountId,
                FestivalId = request.FestivalId,
                Name = request.Name ,
                Balance = request.Balance,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<AccountFestivalWallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();

            return wallet;
        }


        public async Task<List<AccountFestivalWallet>> SearchAccountFestivalWalletsAsync(
     int? accountFestivalWalletId, int? accountId, int? festivalId, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<AccountFestivalWallet>().GetAll()
                .Where(x =>
                    (!accountFestivalWalletId.HasValue || x.AccountFestivalWalletId == accountFestivalWalletId) &&
                    (!accountId.HasValue || x.AccountId == accountId) &&
                    (!festivalId.HasValue || x.FestivalId == festivalId)
                );

            return await query.ToListAsync();
        }
        public async Task<AccountFestivalWallet> UpdateAccountFestivalWalletAsync(int id, decimal newBalance, string? newName)
        {
            var wallet = await _unitOfWork.Repository<AccountFestivalWallet>()
                .FindAsync(x => x.AccountFestivalWalletId == id);

            if (wallet == null)
                throw new KeyNotFoundException($"Không tìm thấy AccountFestivalWallet ID: {id}");

            wallet.Balance = newBalance;
            wallet.Name = newName ?? wallet.Name;
            wallet.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return wallet;
        }
        public async Task DeleteAccountFestivalWalletAsync(int id)
        {
            var wallet = await _unitOfWork.Repository<AccountFestivalWallet>()
                .FindAsync(x => x.AccountFestivalWalletId == id);

            if (wallet == null)
                throw new KeyNotFoundException($"Không tìm thấy AccountFestivalWallet ID: {id}");

            _unitOfWork.Repository<AccountFestivalWallet>().Delete(wallet);
            await _unitOfWork.CommitAsync();
        }

        public async Task TransferToAccountFestivalWalletAsync(WalletTransferRequest request)
        {
            var walletRepo = _unitOfWork.Repository<Wallet>();
            var accountFestivalWalletRepo = _unitOfWork.Repository<AccountFestivalWallet>();

            var wallet = await walletRepo.FindAsync(x => x.WalletId == request.WalletId);
            var afw = await accountFestivalWalletRepo.FindAsync(x => x.AccountFestivalWalletId == request.AccountFestivalWalletId);

            if (wallet == null)
                throw new KeyNotFoundException($"Không tìm thấy Wallet với ID: {request.WalletId}");

            if (afw == null)
                throw new KeyNotFoundException($"Không tìm thấy AccountFestivalWallet với ID: {request.AccountFestivalWalletId}");

            if (wallet.Balance < request.Amount)
                throw new InvalidOperationException("Số dư ví không đủ để thực hiện giao dịch.");

            wallet.Balance -= request.Amount;
            afw.Balance += request.Amount;

            wallet.UpdateAt = DateTime.UtcNow;
            afw.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
        }

        public async Task TransferToMainWalletAsync(WalletTransferRequest request)
        {
            var walletRepo = _unitOfWork.Repository<Wallet>();
            var accountFestivalWalletRepo = _unitOfWork.Repository<AccountFestivalWallet>();

            var wallet = await walletRepo.FindAsync(x => x.WalletId == request.WalletId);
            var afw = await accountFestivalWalletRepo.FindAsync(x => x.AccountFestivalWalletId == request.AccountFestivalWalletId);

            if (wallet == null)
                throw new KeyNotFoundException($"Không tìm thấy Wallet với ID: {request.WalletId}");

            if (afw == null)
                throw new KeyNotFoundException($"Không tìm thấy AccountFestivalWallet với ID: {request.AccountFestivalWalletId}");

            if (afw.Balance < request.Amount)
                throw new InvalidOperationException("Số dư AccountFestivalWallet không đủ để rút.");

            afw.Balance -= request.Amount;
            wallet.Balance += request.Amount;

            wallet.UpdateAt = DateTime.UtcNow;
            afw.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
        }
    }
}
