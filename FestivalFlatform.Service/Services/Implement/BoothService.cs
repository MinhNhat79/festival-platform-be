using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using Microsoft.EntityFrameworkCore;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using FestivalFlatform.Service.Exceptions;
using System.Net;

namespace FestivalFlatform.Service.Services.Implement
{
    public class BoothService : IBoothService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public BoothService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Booth> CreateBoothAsync(BoothCreateRequest request)
        {
            var groupExists = await _unitOfWork.Repository<StudentGroup>()
                .AnyAsync(g => g.GroupId == request.GroupId);
            if (!groupExists)
                throw new CrudException(HttpStatusCode.NotFound, "GroupId không tồn tại", request.GroupId.ToString());

            var hasHomeroomTeacher = await _unitOfWork.Repository<GroupMember>()
                .AnyAsync(gm => gm.GroupId == request.GroupId && gm.Role == "homeroom_teacher");

            if (!hasHomeroomTeacher)
                throw new CrudException(HttpStatusCode.BadRequest,
                    "Nhóm này chưa có giáo viên chủ nhiệm (homeroom_teacher), không thể đăng ký gian hàng",
                    request.GroupId.ToString());

            var festival = await _unitOfWork.Repository<Festival>()
                .FindAsync(f => f.FestivalId == request.FestivalId);

            if (festival == null)
                throw new CrudException(HttpStatusCode.NotFound, "FestivalId không tồn tại", request.FestivalId.ToString());

            if (!festival.Status.Equals("published", StringComparison.OrdinalIgnoreCase))
                throw new CrudException(HttpStatusCode.BadRequest,
                    "Không thể đăng ký gian hàng lễ hội đang diễn ra",
                    request.FestivalId.ToString());

            var locationExists = await _unitOfWork.Repository<MapLocation>()
                .AnyAsync(l => l.LocationId == request.LocationId);
            if (!locationExists)
                throw new CrudException(HttpStatusCode.NotFound, "LocationId không tồn tại", request.LocationId.ToString());

            var alreadyRegistered = await _unitOfWork.Repository<Booth>()
                .AnyAsync(b => b.GroupId == request.GroupId && b.FestivalId == request.FestivalId);

            if (alreadyRegistered)
                throw new CrudException(HttpStatusCode.BadRequest,
                    "Nhóm của bạn đã đăng kí tham gia lễ hội này rồi",
                    request.GroupId.ToString());

            if (request.BoothType.Equals("food", StringComparison.OrdinalIgnoreCase))
            {
                if (festival.RegisteredFoodBooths >= festival.MaxFoodBooths)
                    throw new CrudException(HttpStatusCode.BadRequest, "Số lượng gian hàng Food đã đầy", request.FestivalId.ToString());

                festival.RegisteredFoodBooths++;
            }
            else if (request.BoothType.Equals("beverage", StringComparison.OrdinalIgnoreCase))
            {
                if (festival.RegisteredBeverageBooths >= festival.MaxBeverageBooths)
                    throw new CrudException(HttpStatusCode.BadRequest, "Số lượng gian hàng Beverage đã đầy", request.FestivalId.ToString());

                festival.RegisteredBeverageBooths++;
            }
            else
            {
                throw new CrudException(HttpStatusCode.BadRequest, "BoothType không hợp lệ, chỉ được 'food' hoặc 'beverage'", request.BoothType);
            }

            var booth = new Booth
            {
                GroupId = request.GroupId,
                FestivalId = request.FestivalId,
                LocationId = request.LocationId,
                BoothName = request.BoothName,
                BoothType = request.BoothType,
                Description = request.Description,
                Status = StatusBooth.Pending,
                RegistrationDate = DateTime.UtcNow,
                PointsBalance = 0,
                UpdatedAt = null
            };

            await _unitOfWork.Repository<Booth>().InsertAsync(booth);

        

            await _unitOfWork.CommitAsync();

            return booth;
        }



        public async Task<Booth?> UpdateBoothApproveAsync(int boothId, DateTime approvalDate, int pointsBalance)
        {
            var booth = await _unitOfWork.Repository<Booth>()
                .GetAll()
                .FirstOrDefaultAsync(b => b.BoothId == boothId);

            if (booth == null)
                return null;

          
            if (booth.LocationId == null)
                throw new CrudException(HttpStatusCode.BadRequest, "Booth chưa được gán vị trí", boothId.ToString());

            var mapLocation = await _unitOfWork.Repository<MapLocation>()
                .GetAll()
                .FirstOrDefaultAsync(m => m.LocationId == booth.LocationId);

            if (mapLocation == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy vị trí trên bản đồ", booth.LocationId.ToString());

           
            if (mapLocation.IsOccupied)
                throw new CrudException(HttpStatusCode.BadRequest, "Vị trí này đã có gian hàng khác đặt chỗ. Vui lòng chọn vị trí khác!");

         
            booth.ApprovalDate = approvalDate;
            booth.PointsBalance = pointsBalance;
            booth.UpdatedAt = DateTime.UtcNow;
            booth.Status = StatusBooth.Approved;

           
            

            await _unitOfWork.CommitAsync();

            return booth;
        }


        public async Task<List<Booth>> GetBooths(
            int? boothId,
            int? groupId,
            int? festivalId,
            int? locationId,
            string? boothType,
            string? status,
            int? pageNumber,
            int? pageSize)
        {
            var query = _unitOfWork.Repository<Booth>().GetAll()
                .Include(b => b.Location)           
                .Include(b => b.BoothMenuItems)      
                .AsQueryable();

            
            query = query
                .Where(b => !boothId.HasValue || boothId == 0 || b.BoothId == boothId.Value)
                .Where(b => !groupId.HasValue || groupId == 0 || b.GroupId == groupId.Value)
                .Where(b => !festivalId.HasValue || festivalId == 0 || b.FestivalId == festivalId.Value)
                .Where(b => !locationId.HasValue || locationId == 0 || b.LocationId == locationId.Value)
                .Where(b => string.IsNullOrWhiteSpace(boothType) ||
                            (b.BoothType != null && b.BoothType.Contains(boothType.Trim())))
                .Where(b => string.IsNullOrWhiteSpace(status) || b.Status == status.Trim());

           
            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize)
            //             .Take(currentSize);

            var booths = await query.ToListAsync();

            return booths;
        }

        public async Task UpdateBoothStatusToRejected(int boothId, string? rejectReason)
        {
            var booth = await _unitOfWork.Repository<Booth>().GetAll()
                .FirstOrDefaultAsync(b => b.BoothId == boothId);

            if (booth == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gian hàng", boothId.ToString());
            }
            booth.RejectionReason = rejectReason;
            booth.Status = StatusBooth.Rejected;
            booth.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Booth>().Update(booth, boothId);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateBoothStatusToActive(int boothId)
        {
            var booth = await _unitOfWork.Repository<Booth>().GetAll()
                .FirstOrDefaultAsync(b => b.BoothId == boothId);

            if (booth == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gian hàng", boothId.ToString());
            }

            booth.Status = StatusBooth.Active;
            booth.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Booth>().Update(booth, boothId);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteBoothAsync(int boothId)
        {
            var booth = await _unitOfWork.Repository<Booth>().GetAll()
                .FirstOrDefaultAsync(b => b.BoothId == boothId);

            if (booth == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gian hàng", boothId.ToString());
            }

            _unitOfWork.Repository<Booth>().Delete(booth);
            await _unitOfWork.CommitAsync();
        }

        public async Task<Booth?> UpdateBoothAsync(int boothId, BoothUpdateRequest request)
        {
            var booth = await _unitOfWork.Repository<Booth>()
                .GetAll()
                .Include(b => b.Location)
                .Include(b => b.BoothMenuItems)
                .FirstOrDefaultAsync(b => b.BoothId == boothId);

            if (booth == null)
                return null;

            // Cập nhật thông tin booth
            if (!string.IsNullOrWhiteSpace(request.BoothName))
                booth.BoothName = request.BoothName;
            if (!string.IsNullOrWhiteSpace(request.BoothType))
                booth.BoothType = request.BoothType;
            if (!string.IsNullOrWhiteSpace(request.Description))
                booth.Description = request.Description;
            if (!string.IsNullOrWhiteSpace(request.Status))
                booth.Status = request.Status.Trim();
            if (!string.IsNullOrWhiteSpace(request.IsWithdraw))
                booth.IsWithdraw = request.IsWithdraw.Trim();

            booth.UpdatedAt = DateTime.UtcNow;

            // Cập nhật location nếu có
            if (request.Location != null)
            {
                var location = await _unitOfWork.Repository<MapLocation>()
                                .GetAll()
                                .FirstOrDefaultAsync(l => l.LocationId == request.Location.LocationId);

                if (location != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.Location.LocationName))
                        location.LocationName = request.Location.LocationName;
                    if (!string.IsNullOrWhiteSpace(request.Location.LocationType))
                        location.LocationType = request.Location.LocationType;
                    if (!string.IsNullOrWhiteSpace(request.Location.Coordinates))
                        location.Coordinates = request.Location.Coordinates;
                    if (request.Location.IsOccupied.HasValue)
                        location.IsOccupied = request.Location.IsOccupied.Value;


                    location.UpdatedAt = DateTime.UtcNow;
                    booth.LocationId = location.LocationId;
                }
            }

            // Cập nhật BoothMenuItems nếu có
            if (request.BoothMenuItems != null && request.BoothMenuItems.Any())
            {
                foreach (var itemReq in request.BoothMenuItems)
                {
                    var boothMenuItem = booth.BoothMenuItems
                        .FirstOrDefault(bmi => bmi.BoothMenuItemId == itemReq.BoothMenuItemId);

                    if (boothMenuItem != null)
                    {
                        if (itemReq.CustomPrice.HasValue)
                            boothMenuItem.CustomPrice = itemReq.CustomPrice.Value;
                        if (itemReq.QuantityLimit.HasValue)
                            boothMenuItem.QuantityLimit = itemReq.QuantityLimit.Value;
                        if (!string.IsNullOrWhiteSpace(itemReq.Status))
                            boothMenuItem.Status = itemReq.Status.Trim();
                        if (!string.IsNullOrWhiteSpace(itemReq.ImageUrl))
                        {
                            boothMenuItem.Image = new Image
                            {
                                ImageUrl = itemReq.ImageUrl,
                                BoothId = booth.BoothId,
                                CreatedAt = DateTime.UtcNow
                            };
                        }
                    }
                }
            }

            await _unitOfWork.CommitAsync();

            // Lấy lại booth đã cập nhật
            var updatedBooth = await _unitOfWork.Repository<Booth>()
                .GetAll()
                .Include(b => b.Location)
                .Include(b => b.BoothMenuItems)
                .ThenInclude(bmi => bmi.Image)
                .FirstOrDefaultAsync(b => b.BoothId == boothId);

            return updatedBooth;
        }
        public async Task<Booth> WithdrawBoothRevenueAsync(int boothId, int accountId)
        {
            // 1. Lấy booth
            var booth = await _unitOfWork.Repository<Booth>()
            .GetAll()
            .Include(b => b.Festival)
            .ThenInclude(f => f.FestivalCommission) // include navigation con
        .FirstOrDefaultAsync(b => b.BoothId == boothId);

            if (booth == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gian hàng", boothId.ToString());

            //var festival = booth.Festival;

            //if (festival == null || festival.FestivalCommission == null)
            //    throw new CrudException(HttpStatusCode.BadRequest,
            //        "Vui lòng đợi Admin rút hoa hồng trước khi chuyển doanh thu gian hàng về ví của bạn!");
            // 2. Lấy BoothWallet
            var boothWallet = await _unitOfWork.Repository<BoothWallet>()
                .GetAll()
                .FirstOrDefaultAsync(w => w.BoothId == boothId);

            if (boothWallet == null)
                throw new CrudException(HttpStatusCode.NotFound, "BoothWallet không tồn tại", boothId.ToString());

            if (boothWallet.TotalBalance <= 0)
                throw new CrudException(HttpStatusCode.BadRequest, "BoothWallet không có số dư để rút", boothId.ToString());

            decimal amountToTransfer = boothWallet.TotalBalance;

           
            var accountWallet = await _unitOfWork.Repository<Wallet>()
                        .GetAll()
                        .FirstOrDefaultAsync(w => w.AccountId == accountId);

            if (accountWallet == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "AccountWallet không tồn tại cho accountId " + accountId);
            }

           
            accountWallet.Balance += amountToTransfer;
            accountWallet.UpdateAt = DateTime.UtcNow;

            boothWallet.TotalBalance = 0;
            boothWallet.UpdatedAt = DateTime.UtcNow;

           
            booth.IsWithdraw = "true";
            booth.UpdatedAt = DateTime.UtcNow;

           
            await _unitOfWork.CommitAsync();

          
            return booth;
        }
        public async Task<bool> CanWithdrawRevenueAsync(int boothId, int accountId)
        {
            // 1. Lấy booth và festival
            var booth = await _unitOfWork.Repository<Booth>()
    .GetAll()
    .Include(b => b.Festival)
        .ThenInclude(f => f.FestivalCommission) 
    .FirstOrDefaultAsync(b => b.BoothId == boothId);


            if (booth == null)
                throw new CrudException(HttpStatusCode.NotFound, "Booth không tồn tại", boothId.ToString());

            var festival = booth.Festival;
            if (festival == null || festival.FestivalCommission == null)
                throw new CrudException(HttpStatusCode.BadRequest,
                    "Vui lòng đợi Admin rút hoa hồng trước khi chuyển doanh thu gian hàng về ví của bạn!");

            // 2. Lấy BoothWallet
            var boothWallet = await _unitOfWork.Repository<BoothWallet>()
                .GetAll()
                .FirstOrDefaultAsync(w => w.BoothId == boothId);

            if (boothWallet == null || boothWallet.TotalBalance <= 0)
                throw new CrudException(HttpStatusCode.BadRequest,
                    "số dư của dan hàng đang là 0 nên không thể rút");

            // 3. Lấy AccountWallet
            var accountWallet = await _unitOfWork.Repository<Wallet>()
                .GetAll()
                .FirstOrDefaultAsync(w => w.AccountId == accountId);

            if (accountWallet == null)
                throw new CrudException(HttpStatusCode.BadRequest,
                    "Vui lòng đợi Admin rút hoa hồng trước khi chuyển doanh thu gian hàng về ví của bạn!");

            // 4. Kiểm tra IsWithdraw
            if (booth.IsWithdraw != null && booth.IsWithdraw.Equals("true", StringComparison.OrdinalIgnoreCase))
                throw new CrudException(HttpStatusCode.BadRequest,
                    "Vui lòng đợi Admin rút hoa hồng trước khi chuyển doanh thu gian hàng về ví của bạn!");

            return true;
        }


    }
}
