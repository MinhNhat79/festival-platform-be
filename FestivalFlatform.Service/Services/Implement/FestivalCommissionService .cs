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
    public class FestivalCommissionService : IFestivalCommissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FestivalCommissionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<FestivalCommission> CreateAsync(FestivalCommissionCreateRequest request)
        {
            var festival = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .FirstOrDefaultAsync(f => f.FestivalId == request.FestivalId);

            if (festival == null)
                throw new CrudException(HttpStatusCode.NotFound, "Festival không tồn tại", request.FestivalId.ToString());

            if (await _unitOfWork.Repository<FestivalCommission>()
                .GetAll()
                .AnyAsync(fc => fc.FestivalId == request.FestivalId))
                throw new CrudException(HttpStatusCode.Conflict, "Festival đã có Commission", request.FestivalId.ToString());

            var commission = new FestivalCommission
            {
                FestivalId = request.FestivalId,
                Amount = request.Amount,
                CommissionRate = request.CommissionRate,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<FestivalCommission>().InsertAsync(commission);
            await _unitOfWork.CommitAsync();

            return commission;
        }

        public async Task<FestivalCommission> UpdateAsync(int commissionId, decimal? amount, double? commissionRate)
        {
            var commission = await _unitOfWork.Repository<FestivalCommission>()
                .FindAsync(fc => fc.CommissionId == commissionId);

            if (commission == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Commission", commissionId.ToString());

            if (amount.HasValue)
                commission.Amount = amount.Value;

            if (commissionRate.HasValue)
                commission.CommissionRate = commissionRate.Value;

            commission.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return commission;
        }
        public async Task<List<FestivalCommission>> SearchFestivalCommissionsAsync(
       int? commissionId,
       int? festivalId,
       int? pageNumber,
       int? pageSize)
        {
            var query = _unitOfWork.Repository<FestivalCommission>()
                .GetAll()
                .Include(fc => fc.Festival)
                .AsQueryable();

            query = query
                .Where(fc => !commissionId.HasValue || commissionId == 0 || fc.CommissionId == commissionId.Value)
                .Where(fc => !festivalId.HasValue || festivalId == 0 || fc.FestivalId == festivalId.Value);

            // phân trang (giữ comment lại để sau cần thì mở)
            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize)
            //             .Take(currentSize);

            var commissions = await query
                .OrderByDescending(fc => fc.CreatedAt)
                .ToListAsync();

            return commissions;
        }

        public async Task DeleteAsync(int commissionId)
        {
            var commission = await _unitOfWork.Repository<FestivalCommission>()
                .FindAsync(fc => fc.CommissionId == commissionId);

            if (commission == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Commission", commissionId.ToString());

            _unitOfWork.Repository<FestivalCommission>().Delete(commission);
            await _unitOfWork.CommitAsync();
        }
    }
}
