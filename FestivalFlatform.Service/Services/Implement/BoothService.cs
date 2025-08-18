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
            {
                throw new CrudException(HttpStatusCode.NotFound, "GroupId không tồn tại", request.GroupId.ToString());
            }

            var festivalExists = await _unitOfWork.Repository<Festival>()
                .AnyAsync(f => f.FestivalId == request.FestivalId);
            if (!festivalExists)
            {
                throw new CrudException(HttpStatusCode.NotFound, "FestivalId không tồn tại", request.FestivalId.ToString());
            }

            var locationExists = await _unitOfWork.Repository<MapLocation>()
                .AnyAsync(l => l.LocationId == request.LocationId);
            if (!locationExists)
            {
                throw new CrudException(HttpStatusCode.NotFound, "LocationId không tồn tại", request.LocationId.ToString());
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

        public async Task<Booth?> UpdateBoothAsync(int boothId, DateTime approvalDate, int pointsBalance)
        {
            var booth = await _unitOfWork.Repository<Booth>().FindAsync(b => b.BoothId == boothId);


            if (booth == null)
                return null;

            booth.ApprovalDate = approvalDate;
            booth.PointsBalance = pointsBalance;
            booth.UpdatedAt = DateTime.UtcNow;
            booth.Status = StatusBooth.Approved;

            // EF Core tự tracking, chỉ cần Commit
            await _unitOfWork.CommitAsync();

            return booth;
        }

        public async Task<List<Booth>> GetBooths(int? boothId, int? groupId, int? festivalId, int? locationId, string? boothType, string? status, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Booth>().GetAll()

                .Where(b => !boothId.HasValue || boothId == 0 || b.BoothId == boothId.Value)
                .Where(b => !groupId.HasValue || groupId == 0 || b.GroupId == groupId.Value)
                .Where(b => !festivalId.HasValue || festivalId == 0 || b.FestivalId == festivalId.Value)
                .Where(b => !locationId.HasValue || locationId == 0 || b.LocationId == locationId.Value)
                .Where(b => string.IsNullOrWhiteSpace(boothType) || b.BoothType != null && b.BoothType.Contains(boothType.Trim()))
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
    }
}
