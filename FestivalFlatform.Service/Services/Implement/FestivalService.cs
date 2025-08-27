using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FestivalFlatform.Service.Services.Implement
{
    public class FestivalService : IFestivalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<FestivalService> _logger;


        public FestivalService(IMapper mapper, IUnitOfWork unitOfWork, ILogger<FestivalService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Festival> CreateFestivalAsync(FestivalCreateRequest request)
        {
            // ✅ Validate school
            var schoolExists = await _unitOfWork.Repository<School>()
                .AnyAsync(g => g.SchoolId == request.OrganizerSchoolId);
            if (!schoolExists)
                throw new CrudException(HttpStatusCode.NotFound, "SchoolId không tồn tại", request.OrganizerSchoolId.ToString());

            // ✅ Khởi tạo festival
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
                UpdatedAt = null,
                TotalRevenue = 0m,
                Description = request.Description
            };

            // ✅ Insert Festival
            await _unitOfWork.Repository<Festival>().InsertAsync(festival);

            // ✅ Commit
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
            var festival = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .Include(f => f.Images)
                .Include(f => f.Booths)
                .Include(f => f.FestivalIngredients)
                .Include(f => f.FestivalMaps)
                    .ThenInclude(m => m.Locations)
                .Include(f => f.FestivalMenus)
                    .ThenInclude(m => m.MenuItems)
                .Include(f => f.FestivalSchools)
                .Include(f => f.AccountFestivalWallets)
                .FirstOrDefaultAsync(f => f.FestivalId == festivalId);

            if (festival == null)
                throw new CrudException(HttpStatusCode.NotFound, "Festival không tồn tại", festivalId.ToString());

            // Xoá con trước
            if (festival.Images.Any())
                _unitOfWork.Repository<Image>().DeleteRange(festival.Images.AsQueryable());

            if (festival.Booths.Any())
                _unitOfWork.Repository<Booth>().DeleteRange(festival.Booths.AsQueryable());

            if (festival.FestivalIngredients.Any())
                _unitOfWork.Repository<FestivalIngredient>().DeleteRange(festival.FestivalIngredients.AsQueryable());

            if (festival.FestivalMaps.Any())
            {
                foreach (var map in festival.FestivalMaps)
                {
                    if (map.Locations.Any())
                        _unitOfWork.Repository<MapLocation>().DeleteRange(map.Locations.AsQueryable());
                }
                _unitOfWork.Repository<FestivalMap>().DeleteRange(festival.FestivalMaps.AsQueryable());
            }

            if (festival.FestivalMenus.Any())
            {
                foreach (var menu in festival.FestivalMenus)
                {
                    if (menu.MenuItems.Any())
                        _unitOfWork.Repository<MenuItem>().DeleteRange(menu.MenuItems.AsQueryable());
                }
                _unitOfWork.Repository<FestivalMenu>().DeleteRange(festival.FestivalMenus.AsQueryable());
            }

            if (festival.FestivalSchools.Any())
                _unitOfWork.Repository<FestivalSchool>().DeleteRange(festival.FestivalSchools.AsQueryable());

            if (festival.AccountFestivalWallets.Any())
                _unitOfWork.Repository<AccountFestivalWallet>().DeleteRange(festival.AccountFestivalWallets.AsQueryable());


            // Cuối cùng xoá Festival
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

            // 5. Lấy Wallet của admin (RoleId = 1)
            var adminWallet = await _unitOfWork.Repository<Wallet>()
                .GetAll()
                .Include(w => w.Account)
                .FirstOrDefaultAsync(w => w.Account.RoleId == 1);

            if (adminWallet == null)
                throw new InvalidOperationException("Admin wallet not found");

            // Cộng hoa hồng vào Wallet của admin
            adminWallet.Balance += commissionAmount;
            adminWallet.UpdateAt = DateTime.UtcNow;

            // 6. Trừ tiền từ BoothWallets (chia đều)
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

        public async Task testdate()
        {
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                   TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var todayVN = vietnamTime.Date;
             _logger.LogInformation("✅ FestivalJobService: Updated status for festivals on {Date}", todayVN);
        }
        public async Task UpdateFestivalStatusDailyAsync()
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                // Giờ hiện tại theo VN
                var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                var todayVN = vietnamTime.Date;

                var festivals = await _unitOfWork.Repository<Festival>()
                    .GetAll()
                    .Where(f => f.Status == StatusFestival.Published || f.Status == StatusFestival.Ongoing)
                    .ToListAsync();

                foreach (var fest in festivals)
                {
                    if (fest.StartDate.HasValue)
                    {
                        var startVN = TimeZoneInfo.ConvertTimeFromUtc(fest.StartDate.Value, vietnamTimeZone).Date;
                        if (fest.Status == StatusFestival.Published && startVN == todayVN)
                        {
                            fest.Status = StatusFestival.Ongoing;
                            fest.UpdatedAt = DateTime.UtcNow; // lưu UTC
                        }
                    }

                    if (fest.EndDate.HasValue)
                    {
                        var endVN = TimeZoneInfo.ConvertTimeFromUtc(fest.EndDate.Value, vietnamTimeZone).Date;
                        if (fest.Status == StatusFestival.Ongoing && endVN == todayVN)
                        {
                            fest.Status = StatusFestival.Completed;
                            fest.UpdatedAt = DateTime.UtcNow; // lưu UTC
                        }
                    }
                }

                await _unitOfWork.CommitAsync();
                _logger.LogInformation("✅ FestivalJobService: Updated festival statuses for {Date}", todayVN);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in FestivalJobService.UpdateFestivalStatusDailyAsync");
            }
        }



        public async Task<Festival?> GetFestivalDetailAsync(int festivalId)
        {
            var festival = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .Include(f => f.Images)
                .Include(f => f.FestivalMaps)
                    .ThenInclude(m => m.Locations)
                .Include(f => f.FestivalMenus)
                    .ThenInclude(m => m.MenuItems)
                .FirstOrDefaultAsync(f => f.FestivalId == festivalId);

            if (festival == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Festival", festivalId.ToString());
            }

            return festival;
        }


        public async Task<Festival> UpdateFestivalInfoAsync(UpdateFestivalRequest request)
        {
            var festival = await _unitOfWork.Repository<Festival>()
        .GetAll()
        .Include(f => f.Images)
        .Include(f => f.FestivalMaps).ThenInclude(m => m.Locations)
        .Include(f => f.FestivalMenus).ThenInclude(m => m.MenuItems)
        .FirstOrDefaultAsync(f => f.FestivalId == request.FestivalId);

            if (festival == null)
                throw new KeyNotFoundException("Festival không tồn tại");
            if (!string.Equals(festival.Status, "draft", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ có thể cập nhật festival khi đang ở trạng thái draft");
            // 🎯 Update Festival fields
            festival.SchoolId = request.SchoolId ?? festival.SchoolId;
            festival.FestivalName = request.FestivalName ?? festival.FestivalName;
            festival.Theme = request.Theme ?? festival.Theme;
            festival.StartDate = request.StartDate ?? festival.StartDate;
            festival.EndDate = request.EndDate ?? festival.EndDate;
            festival.RegistrationStartDate = request.RegistrationStartDate ?? festival.RegistrationStartDate;
            festival.RegistrationEndDate = request.RegistrationEndDate ?? festival.RegistrationEndDate;
            festival.Location = request.Location ?? festival.Location;
            festival.MaxFoodBooths = request.MaxFoodBooths ?? festival.MaxFoodBooths;
            festival.MaxBeverageBooths = request.MaxBeverageBooths ?? festival.MaxBeverageBooths;
            festival.RegisteredFoodBooths = request.RegisteredFoodBooths ?? festival.RegisteredFoodBooths;
            festival.RegisteredBeverageBooths = request.RegisteredBeverageBooths ?? festival.RegisteredBeverageBooths;
            festival.Status = request.Status ?? festival.Status;
            
            festival.Description = request.Description ?? festival.Description;
            festival.cancellationReason = request.CancellationReason;
            festival.UpdatedAt = DateTime.UtcNow;

            // 🎯 Update Images
            if (request.Images != null)
            {
                // Xóa image không còn trong request
                var removeImages = festival.Images
                    .Where(i => !request.Images.Any(ri => ri.ImageId == i.ImageId))
                    .ToList();
                foreach (var img in removeImages)
                    _unitOfWork.Repository<Image>().Delete(img);

                // Update / Insert
                foreach (var imgReq in request.Images)
                {
                    var existing = festival.Images.FirstOrDefault(i => i.ImageId == imgReq.ImageId);
                    if (existing != null)
                    {
                        existing.ImageUrl = imgReq.ImageUrl;
                        existing.ImageName = imgReq.ImageName;
                        existing.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        imgReq.FestivalId = festival.FestivalId;
                        await _unitOfWork.Repository<Image>().InsertAsync(imgReq);
                    }
                }
            }

            // 🎯 Update FestivalMaps + Locations
            if (request.FestivalMaps != null)
            {
                var removeMaps = festival.FestivalMaps
                    .Where(m => !request.FestivalMaps.Any(rm => rm.MapId == m.MapId))
                    .ToList();
                foreach (var map in removeMaps)
                    _unitOfWork.Repository<FestivalMap>().Delete(map);

                foreach (var mapReq in request.FestivalMaps)
                {
                    var existingMap = festival.FestivalMaps.FirstOrDefault(m => m.MapId == mapReq.MapId);
                    if (existingMap != null)
                    {
                        existingMap.MapName = mapReq.MapName;
                        existingMap.MapType = mapReq.MapType;
                        existingMap.MapUrl = mapReq.MapUrl;
                        existingMap.LastUpdated = DateTime.UtcNow;

                        // Update locations
                        var removeLocs = existingMap.Locations
                            .Where(l => !mapReq.Locations.Any(rl => rl.LocationId == l.LocationId))
                            .ToList();
                        foreach (var loc in removeLocs)
                            _unitOfWork.Repository<MapLocation>().Delete(loc);

                        foreach (var locReq in mapReq.Locations)
                        {
                            var existingLoc = existingMap.Locations.FirstOrDefault(l => l.LocationId == locReq.LocationId);
                            if (existingLoc != null)
                            {
                                existingLoc.LocationName = locReq.LocationName;
                                existingLoc.LocationType = locReq.LocationType;
                                existingLoc.Coordinates = locReq.Coordinates;
                                existingLoc.IsOccupied = locReq.IsOccupied;
                                existingLoc.UpdatedAt = DateTime.UtcNow;
                            }
                            else
                            {
                                locReq.MapId = existingMap.MapId;
                                await _unitOfWork.Repository<MapLocation>().InsertAsync(locReq);
                            }
                        }
                    }
                    else
                    {
                        mapReq.FestivalId = festival.FestivalId;
                        await _unitOfWork.Repository<FestivalMap>().InsertAsync(mapReq);
                    }
                }
            }

            // 🎯 Update FestivalMenus + MenuItems
            if (request.FestivalMenus != null)
            {
                var removeMenus = festival.FestivalMenus
                    .Where(m => !request.FestivalMenus.Any(rm => rm.MenuId == m.MenuId))
                    .ToList();
                foreach (var menu in removeMenus)
                    _unitOfWork.Repository<FestivalMenu>().Delete(menu);

                foreach (var menuReq in request.FestivalMenus)
                {
                    var existingMenu = festival.FestivalMenus.FirstOrDefault(m => m.MenuId == menuReq.MenuId);
                    if (existingMenu != null)
                    {
                        existingMenu.MenuName = menuReq.MenuName;
                        existingMenu.Description = menuReq.Description;
                        existingMenu.UpdatedAt = DateTime.UtcNow;

                        // Update menu items
                        var removeItems = existingMenu.MenuItems
                            .Where(i => !menuReq.MenuItems.Any(ri => ri.ItemId == i.ItemId))
                            .ToList();
                        foreach (var item in removeItems)
                            _unitOfWork.Repository<MenuItem>().Delete(item);

                        foreach (var itemReq in menuReq.MenuItems)
                        {
                            var existingItem = existingMenu.MenuItems.FirstOrDefault(i => i.ItemId == itemReq.ItemId);
                            if (existingItem != null)
                            {
                                existingItem.ItemName = itemReq.ItemName;
                                existingItem.Description = itemReq.Description;
                                existingItem.ItemType = itemReq.ItemType;
                                existingItem.MinPrice = itemReq.MinPrice;
                                existingItem.MaxPrice = itemReq.MaxPrice;
                                existingItem.Status = itemReq.Status;
                                existingItem.UpdatedAt = DateTime.UtcNow;
                            }
                            else
                            {
                                itemReq.MenuId = existingMenu.MenuId;
                                await _unitOfWork.Repository<MenuItem>().InsertAsync(itemReq);
                            }
                        }
                    }
                    else
                    {
                        menuReq.FestivalId = festival.FestivalId;
                        await _unitOfWork.Repository<FestivalMenu>().InsertAsync(menuReq);
                    }
                }
            }

            await _unitOfWork.CommitAsync();
            return festival;
        }

    }
}
