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

            var groupExists = await _unitOfWork.Repository<School>()
           .AnyAsync(g => g.SchoolId == request.OrganizerSchoolId);
            if (!groupExists)
            {
                throw new CrudException(HttpStatusCode.NotFound, "SchoolId không tồn tại", request.OrganizerSchoolId.ToString());
            }


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
                Status = StatusFestival.Draft, // Hoặc chuỗi "draft"
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _unitOfWork.Repository<Festival>().InsertAsync(festival);
            await _unitOfWork.CommitAsync();

            return festival;
        }
        public async Task<Festival> UpdateFestivalAsync(int festivalId, int? maxFoodBooths, int? maxBeverageBooths, int? registeredFoodBooths, int? registeredBeverageBooths, string? status)
        {
            var festival = await _unitOfWork.Repository<Festival>().FindAsync(f => f.FestivalId == festivalId);

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

            if (!string.IsNullOrWhiteSpace(status))
                festival.Status = status.Trim();

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

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

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

    }
}
