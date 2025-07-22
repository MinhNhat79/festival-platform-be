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
    public class SchoolAccountRelationService : ISchoolAccountRelationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public SchoolAccountRelationService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SchoolAccountRelation> CreateRelationAsync(CreateSchoolAccountRelationRequest request)
        {
            var relationType = request.RelationType.Trim().ToLower();

            if (relationType != "teacher" && relationType != "student")
                throw new CrudException(HttpStatusCode.BadRequest, "RelationType phải là 'teacher' hoặc 'student'", relationType);

            var exists = await _unitOfWork.Repository<SchoolAccountRelation>()
                .AnyAsync(x => x.SchoolId == request.SchoolId && x.AccountId == request.AccountId && x.RelationType == relationType);

            if (exists)
                throw new CrudException(HttpStatusCode.Conflict, "Quan hệ đã tồn tại", relationType);

            var entity = new SchoolAccountRelation
            {
                SchoolId = request.SchoolId,
                AccountId = request.AccountId,
                RelationType = relationType,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<SchoolAccountRelation>().InsertAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public async Task<SchoolAccountRelation> UpdateRelationAsync(int id, string relationType)
        {
            var relation = await _unitOfWork.Repository<SchoolAccountRelation>().GetAll()
                .FirstOrDefaultAsync(s => s.Id==id);
            if (relation == null)
                throw new CrudException(HttpStatusCode.NotFound, "Not found", id.ToString());

            relation.RelationType = relationType.Trim().ToLower();
            relation.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return relation;
        }
        public async Task<List<SchoolAccountRelation>> SearchRelationsAsync(int? schoolId, int? accountId, string? relationType)
        {
            var query = _unitOfWork.Repository<SchoolAccountRelation>().GetAll()
                .Where(x => !schoolId.HasValue || x.SchoolId == schoolId.Value)
                .Where(x => !accountId.HasValue || x.AccountId == accountId.Value)
                .Where(x => string.IsNullOrWhiteSpace(relationType) || x.RelationType == relationType.Trim().ToLower());

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteRelationAsync(int id)
        {
            var relation = await _unitOfWork.Repository<SchoolAccountRelation>().GetAll()
                .FirstOrDefaultAsync(s => s.Id ==  id);
            if (relation == null) return false;

            _unitOfWork.Repository<SchoolAccountRelation>().Delete(relation);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
