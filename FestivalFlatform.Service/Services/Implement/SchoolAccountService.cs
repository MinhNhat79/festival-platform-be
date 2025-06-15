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
    public class SchoolAccountService : ISchoolAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public SchoolAccountService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SchoolAccount> CreateSchoolAccountAsync(SchoolAccountCreateRequest request)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.AccountId == request.AccountId);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", request.AccountId.ToString());
            }

            // Kiểm tra RoleId có phải là School (RoleId = 3)
            if (account.RoleId != 2)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Tài khoản không thuộc vai trò Trường ", request.AccountId.ToString());
            }
            var schoolAccount = new SchoolAccount
            {
                SchoolId = request.SchoolId,
                AccountId = request.AccountId,
                Position = request.Position,
                Department = request.Department,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _unitOfWork.Repository<SchoolAccount>().InsertAsync(schoolAccount);
            await _unitOfWork.CommitAsync();

            return schoolAccount;
        }

        public async Task<SchoolAccount> UpdateSchoolAccountAsync(int schoolAccountId, string? position, string? department)
        {
            var schoolAccount = await _unitOfWork.Repository<SchoolAccount>().FindAsync(sa => sa.SchoolAccountId == schoolAccountId);
            if (schoolAccount == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy SchoolAccount", schoolAccountId.ToString());
            }

            schoolAccount.Position = position ?? schoolAccount.Position;
            schoolAccount.Department = department ?? schoolAccount.Department;
            schoolAccount.UpdatedAt = DateTime.UtcNow;


            await _unitOfWork.CommitAsync();

            return schoolAccount;
        }

        public async Task<List<SchoolAccount>> SearchSchoolAccountsAsync(int? schoolAccountId, int? schoolId, int? accountId, string? position, string? department, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<SchoolAccount>().GetAll()
                .Where(sa => !schoolAccountId.HasValue || sa.SchoolAccountId == schoolAccountId.Value)
                .Where(sa => !schoolId.HasValue || sa.SchoolId == schoolId.Value)
                .Where(sa => !accountId.HasValue || sa.AccountId == accountId.Value)
                .Where(sa => string.IsNullOrWhiteSpace(position) || sa.Position != null && sa.Position.Contains(position.Trim()))
                .Where(sa => string.IsNullOrWhiteSpace(department) || sa.Department != null && sa.Department.Contains(department.Trim()));

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            query = query.Skip((currentPage - 1) * currentSize)
                         .Take(currentSize);

            var result = await query.ToListAsync();

            if (result == null || result.Count == 0)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy SchoolAccount phù hợp", schoolAccountId?.ToString() ?? "No filter");
            }

            return result;
        }

        public async Task<bool> DeleteSchoolAccountAsync(int schoolAccountId)
        {
            var schoolAccount = await _unitOfWork.Repository<SchoolAccount>().FindAsync(sa => sa.SchoolAccountId == schoolAccountId);
            if (schoolAccount == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy SchoolAccount", schoolAccountId.ToString());
            }

            _unitOfWork.Repository<SchoolAccount>().Delete(schoolAccount);
            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
