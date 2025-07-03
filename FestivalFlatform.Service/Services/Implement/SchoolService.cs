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
    public class SchoolService : ISchoolService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public SchoolService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<School> CreateSchoolAsync(SchoolCreateRequest request)
        { 
         


            var school = new School
            {
                SchoolName = request.SchoolName,
                AccountId = request.AccountId,
                Address = request.Address,
                ContactInfo = request.ContactInfo,
                LogoUrl = request.LogoUrl,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<School>().InsertAsync(school);
            await _unitOfWork.CommitAsync();

            return school;
        }

        public async Task<School> UpdateSchoolAsync(int schoolId, string? contactInfo, string? logoUrl, string? description)
        {
            var school = await _unitOfWork.Repository<School>().FindAsync(s => s.SchoolId == schoolId);

            if (school == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy trường", schoolId.ToString());
            }

            school.ContactInfo = contactInfo ?? school.ContactInfo;
            school.LogoUrl = logoUrl ?? school.LogoUrl;
            school.Description = description ?? school.Description;
            school.UpdatedAt = DateTime.UtcNow;
      
            await _unitOfWork.CommitAsync();

            return school;
        }
        public async Task<List<School>> SearchSchoolsAsync(int? schoolId, int? accountId, string? schoolName, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<School>().GetAll()
                .Where(s => !schoolId.HasValue || schoolId == 0 || s.SchoolId == schoolId.Value)
                .Where(s => string.IsNullOrWhiteSpace(schoolName) || s.SchoolName.Contains(schoolName.Trim()));

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            query = query.Skip((currentPage - 1) * currentSize)
                         .Take(currentSize);

            var schools = await query.ToListAsync();

         

            return schools;
        }
        public async Task DeleteSchoolAsync(int schoolId)
        {
            var school = await _unitOfWork.Repository<School>().FindAsync(s => s.SchoolId == schoolId);

            if (school == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy trường", schoolId.ToString());
            }

            _unitOfWork.Repository<School>().Delete(school);
            await _unitOfWork.CommitAsync();
        }

    }
}
