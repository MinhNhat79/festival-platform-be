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
    public class FestivalSchoolService : IFestivalSchoolService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public FestivalSchoolService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<FestivalSchool> CreateFestivalSchoolAsync(FestivalSchoolCreateRequest request)
        {
            // Validate Festival
            var festival = await _unitOfWork.Repository<Festival>().FindAsync(f => f.FestivalId == request.FestivalId);
            if (festival == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Festival", request.FestivalId.ToString());
            }

            // Validate School
            var school = await _unitOfWork.Repository<School>().FindAsync(s => s.SchoolId == request.SchoolId);
            if (school == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy School", request.SchoolId.ToString());
            }

            // Check if already exists
            var existing = await _unitOfWork.Repository<FestivalSchool>().GetAll()
                .AnyAsync(fs => fs.FestivalId == request.FestivalId && fs.SchoolId == request.SchoolId);

            if (existing)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "FestivalSchool đã tồn tại", $"{request.FestivalId}-{request.SchoolId}");
            }

            var festivalSchool = new FestivalSchool
            {
                FestivalId = request.FestivalId,
                SchoolId = request.SchoolId,
                Status = StatusFestivalSchool.Pending,
                RegistrationDate = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<FestivalSchool>().InsertAsync(festivalSchool);
            await _unitOfWork.CommitAsync();

            return festivalSchool;
        }
        public async Task<FestivalSchool> UpdateFestivalSchoolAsync(
        int id, int? festivalId, int? schoolId)
        {
            var entity = await _unitOfWork.Repository<FestivalSchool>()
                .GetAll()
                .FirstOrDefaultAsync(fs => fs.FestivalSchoolId == id);

            if (entity == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalSchool", id.ToString());

            if (festivalId.HasValue && entity.FestivalId != festivalId.Value)
            {
                var festivalExists = await _unitOfWork.Repository<Festival>().AnyAsync(f => f.FestivalId == festivalId);
                if (!festivalExists)
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Festival", festivalId.ToString());

                entity.FestivalId = festivalId.Value;
            }

            if (schoolId.HasValue && entity.SchoolId != schoolId.Value)
            {
                var schoolExists = await _unitOfWork.Repository<School>().AnyAsync(s => s.SchoolId == schoolId);
                if (!schoolExists)
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy School", schoolId.ToString());

                entity.SchoolId = schoolId.Value;
            }


            entity.ApprovalDate = DateTime.UtcNow;
            entity.RegistrationDate = entity.RegistrationDate; // giữ nguyên

            await _unitOfWork.CommitAsync();

            return entity;
        }
        public async Task<List<FestivalSchool>> SearchFestivalSchoolsAsync(
        int? festivalSchoolId, int? festivalId, int? schoolId, string? status, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<FestivalSchool>().GetAll()
                .Where(fs => !festivalSchoolId.HasValue || fs.FestivalSchoolId == festivalSchoolId)
                .Where(fs => !festivalId.HasValue || fs.FestivalId == festivalId)
                .Where(fs => !schoolId.HasValue || fs.SchoolId == schoolId)
                .Where(fs => string.IsNullOrWhiteSpace(status) || fs.Status == status.Trim());

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();


            return result;
        }

        public async Task DeleteFestivalSchoolAsync(int id)
        {
            var entity = await _unitOfWork.Repository<FestivalSchool>()
                .GetAll()
                .FirstOrDefaultAsync(fs => fs.FestivalSchoolId == id);

            if (entity == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalSchool", id.ToString());

            _unitOfWork.Repository<FestivalSchool>().Delete(entity);
            await _unitOfWork.CommitAsync();
        }
        public async Task UpdateFestivalSchoolStatusToRejectAsync(int festivalSchoolId)
        {
            var entity = await _unitOfWork.Repository<FestivalSchool>()
                .GetAll()
                .FirstOrDefaultAsync(fs => fs.FestivalSchoolId == festivalSchoolId);

            if (entity == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalSchool", festivalSchoolId.ToString());
            }

            entity.Status = StatusFestivalSchool.Rejected;
            entity.ApprovalDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
        }
        public async Task UpdateFestivalSchoolStatusToApproveAsync(int festivalSchoolId)
        {
            var entity = await _unitOfWork.Repository<FestivalSchool>()
                .GetAll()
                .FirstOrDefaultAsync(fs => fs.FestivalSchoolId == festivalSchoolId);

            if (entity == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalSchool", festivalSchoolId.ToString());
            }

            entity.Status = StatusFestivalSchool.Approved;
            entity.ApprovalDate = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
        }

    }
}
