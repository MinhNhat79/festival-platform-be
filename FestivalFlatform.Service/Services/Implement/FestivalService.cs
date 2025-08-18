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
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class FestivalService : IFestivalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public FestivalService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Festival> CreateFestivalAsync(FestivalCreateRequest request)
        {
            // Validate school
            var schoolExists = await _unitOfWork.Repository<School>()
                .AnyAsync(g => g.SchoolId == request.OrganizerSchoolId);
            if (!schoolExists)
                throw new CrudException(HttpStatusCode.NotFound, "SchoolId không tồn tại", request.OrganizerSchoolId.ToString());

            var festival = new Festival
            {
                SchoolId = request.OrganizerSchoolId,
                FestivalName = request.FestivalName,
                Theme = request.Theme,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RegistrationStartDate = request.RegistrationStartDate,
                RegistrationEndDate = request.RegistrationEndDate,
                Location = request.Location,
                MaxFoodBooths = request.MaxFoodBooths,
                MaxBeverageBooths = request.MaxBeverageBooths,
                RegisteredFoodBooths = 0,
                RegisteredBeverageBooths = 0,
                Status = StatusFestival.Draft,
                CreatedAt = DateTime.UtcNow,
                Description = request.Description,
                UpdatedAt = null,
                TotalRevenue = 0m
            };

            // Add festival (CHƯA commit)
            await _unitOfWork.Repository<Festival>().InsertAsync(festival);

            // Lấy tất cả admin (RoleId = 1)
            var adminIds = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Where(a => a.RoleId == 1)
                .Select(a => a.AccountId)
                .ToListAsync();

            foreach (var adminId in adminIds)
            {
                // Tạo AccountFestivalWallet
                var wallet = new AccountFestivalWallet
                {
                    AccountId = adminId,
                    Festival = festival, // EF tự set FestivalId khi commit
                    Balance = 0m,
                    Name = $"Ví phụ cho {festival.FestivalName}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<AccountFestivalWallet>().InsertAsync(wallet);

                // Tạo AccountWalletHistory cho admin
                var history = new AccountWalletHistory
                {
                    AccountId = adminId,
                    Description = $"Khởi tạo ví phụ cho lễ hội {festival.FestivalName}",
                    Amount = 0m,
                    Type = "init",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<AccountWalletHistory>().InsertAsync(history);
            }

            // Commit MỘT LẦN
            await _unitOfWork.CommitAsync();

            return festival;
        }





        public async Task<Festival> UpdateFestivalAsync(
       int festivalId,
       int? maxFoodBooths,
       int? maxBeverageBooths,
       int? registeredFoodBooths,
       int? registeredBeverageBooths,
       string? cancelReason,
       string? status)
        {
            var festival = await _unitOfWork.Repository<Festival>()
                .FindAsync(f => f.FestivalId == festivalId);

            if (festival == null)
                throw new CrudException(HttpStatusCode.NotFound, "Festival không tồn tại", festivalId.ToString());

            if (maxFoodBooths.HasValue)
                festival.MaxFoodBooths = maxFoodBooths.Value;

            if (maxBeverageBooths.HasValue)
                festival.MaxBeverageBooths = maxBeverageBooths.Value;

            if (registeredFoodBooths.HasValue)
                festival.RegisteredFoodBooths = registeredFoodBooths.Value;

            if (registeredBeverageBooths.HasValue)
                festival.RegisteredBeverageBooths = registeredBeverageBooths.Value;

            if (!string.IsNullOrWhiteSpace(cancelReason))
                festival.cancellationReason = cancelReason.Trim();

            if (!string.IsNullOrWhiteSpace(status))
            {
                festival.Status = status.Trim();

                if (string.Equals(status.Trim(), "completed", StringComparison.OrdinalIgnoreCase))
                {
                    // Lấy tất cả booth theo festivalId
                    var booths = await _unitOfWork.Repository<Booth>()
                        .GetAll()
                        .Where(b => b.FestivalId == festivalId)
                        .ToListAsync();

                    decimal totalRevenue = 0;

                    if (booths.Any())
                    {
                        foreach (var booth in booths)
                        {
                            // Tính doanh thu của booth
                            var boothRevenue = await _unitOfWork.Repository<Order>()
                                .GetAll()
                                .Where(o => o.BoothId == booth.BoothId)
                                .SumAsync(o => o.TotalAmount);

                            // Lấy BoothWallet tương ứng
                            var boothWallet = await _unitOfWork.Repository<BoothWallet>()
                                .FindAsync(w => w.BoothId == booth.BoothId);

                            if (boothWallet != null)
                            {
                                boothWallet.TotalBalance = boothRevenue;
                                boothWallet.UpdatedAt = DateTime.UtcNow;
                            }

                            // Cộng dồn doanh thu vào festival
                            totalRevenue += boothRevenue;
                        }
                    }

                    // Gán vào festival
                    festival.TotalRevenue = totalRevenue;
                }
            }

            festival.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return festival;
        }


        public async Task<List<Festival>> SearchFestivalsAsync(int? festivalId, int? schoolId, string? status,
        DateTime? startDate, DateTime? endDate, DateTime? registrationStartDate, DateTime? registrationEndDate,
        int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Festival>().GetAll()
                .Where(f => !festivalId.HasValue || f.FestivalId == festivalId)
                .Where(f => !schoolId.HasValue || f.SchoolId == schoolId)
                .Where(f => string.IsNullOrWhiteSpace(status) || f.Status == status.Trim())
                .Where(f => !startDate.HasValue || (f.StartDate.HasValue && f.StartDate.Value.Date >= startDate.Value.Date))
                .Where(f => !endDate.HasValue || (f.EndDate.HasValue && f.EndDate.Value.Date <= endDate.Value.Date))
                .Where(f => !registrationStartDate.HasValue || (f.RegistrationStartDate.HasValue && f.RegistrationStartDate.Value.Date >= registrationStartDate.Value.Date))
                .Where(f => !registrationEndDate.HasValue || (f.RegistrationEndDate.HasValue && f.RegistrationEndDate.Value.Date <= registrationEndDate.Value.Date));

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var festivals = await query.ToListAsync();



            return festivals;
        }

        public async Task DeleteFestivalAsync(int festivalId)
        {
            var festival = await _unitOfWork.Repository<Festival>().FindAsync(f => f.FestivalId == festivalId);
            if (festival == null)
                throw new CrudException(HttpStatusCode.NotFound, "Festival không tồn tại", festivalId.ToString());

            _unitOfWork.Repository<Festival>().Delete(festival);
            await _unitOfWork.CommitAsync();
        }
        public async Task<bool> DistributeCommissionAsync(DistributeCommissionRequest request)
        {
            // 1. Kiểm tra festival tồn tại và status = Completed
            var festival = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .FirstOrDefaultAsync(f => f.FestivalId == request.FestivalId);

            if (festival == null)
                throw new ArgumentException("Festival does not exist");

            if (festival.Status != StatusFestival.Completed)
                throw new InvalidOperationException("Festival must be completed before distributing commission");

            // 2. Lấy tất cả BoothWallet thuộc các Booth của festival
            var boothWallets = await _unitOfWork.Repository<BoothWallet>()
                .GetAll()
                .Include(bw => bw.Booth)
                .Where(bw => bw.Booth.FestivalId == request.FestivalId)
                .ToListAsync();

            if (!boothWallets.Any())
                throw new InvalidOperationException("No booth wallets found for this festival");

            // 3. Tính tổng doanh thu
            decimal totalRevenue = boothWallets.Sum(bw => bw.TotalBalance);

            // 4. Tính tiền hoa hồng admin nhận
            decimal commissionAmount = totalRevenue * (request.CommissionRate / 100);

            // 5. Lấy AccountFestivalWallet của admin trong festival này
            var adminAccountId = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Where(a => a.RoleId == 1)
                .Select(a => a.AccountId)
                .FirstOrDefaultAsync();

            if (adminAccountId == 0)
                throw new InvalidOperationException("No admin account found");

            var adminFestivalWallet = await _unitOfWork.Repository<AccountFestivalWallet>()
                .GetAll()
                .FirstOrDefaultAsync(w => w.AccountId == adminAccountId && w.FestivalId == request.FestivalId);

            if (adminFestivalWallet == null)
            {
                // Nếu chưa có thì tạo mới
                adminFestivalWallet = new AccountFestivalWallet
                {
                    AccountId = adminAccountId,
                    FestivalId = request.FestivalId,
                    Balance = commissionAmount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<AccountFestivalWallet>().InsertAsync(adminFestivalWallet);
            }
            else
            {
                // Cộng thêm hoa hồng
                adminFestivalWallet.Balance += commissionAmount;
                adminFestivalWallet.UpdatedAt = DateTime.UtcNow;
            }

            // 6. Trừ tiền từ BoothWallets
            int boothCount = boothWallets.Count;
            decimal deductionPerBooth = commissionAmount / boothCount;

            foreach (var boothWallet in boothWallets)
            {
                boothWallet.TotalBalance -= deductionPerBooth;
                if (boothWallet.TotalBalance < 0)
                    boothWallet.TotalBalance = 0;
                boothWallet.UpdatedAt = DateTime.UtcNow;
            }

            // 7. Lưu thay đổi
            await _unitOfWork.CommitAsync();

            return true;
        }


    }
}
