using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace FestivalFlatform.Service.Services.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public AccountService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        //HashPAss
        private void CreatePasswordHash(string password, out string passwordHash)
        {
            passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<AccountResponse> RegisterAccount(RegisterRequestAll request)
        {
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);

            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", request.Email.ToString());
            }

            var phoneNumberExisted = _unitOfWork.Repository<Account>().Find(x => x.PhoneNumber == request.PhoneNumber);

            if (phoneNumberExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "so dien thaoi đã tồn tại", request.PhoneNumber.ToString());
            }
            CreatePasswordHash(request.Password, out string passwordHash); // tạo password đã mã hóa

            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId,
                PasswordHash = passwordHash, // ✅ dùng mật khẩu đã hash
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullNme = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
            };
        }

        //Create Account Student by SchoolManager
        public async Task<AccountResponse> RegisterStudentAccountBySchoolManager(RegisterRequest request)
        {
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);

            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", request.Email.ToString());
            }


            var phoneNumberExisted = _unitOfWork.Repository<Account>().Find(x => x.PhoneNumber == request.PhoneNumber);

            if (phoneNumberExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "so dien thaoi đã tồn tại");
            }
            var studentRole = await _unitOfWork.Repository<Role>()
            .GetAll()
            .FirstOrDefaultAsync(r => r.RoleName.ToLower() == "Student");

            if (studentRole == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy role 'student'");
            }
            CreatePasswordHash(request.Password, out string passwordHash); // tạo password đã mã hóa

            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = studentRole.RoleId, // học sinh
                PasswordHash = passwordHash, // ✅ dùng mật khẩu đã hash
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullNme = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
            };
        }


        public async Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, int? role, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Account>().GetAll()
                                                         // Lọc theo từng param, nếu param null hoặc rỗng thì bỏ qua
                                                         .Where(a => !id.HasValue || id == 0 || a.AccountId == id.Value)
                                                         .Where(a => string.IsNullOrWhiteSpace(phone) || a.PhoneNumber.Contains(phone.Trim()))
                                                         .Where(a => string.IsNullOrWhiteSpace(email) || a.Email.Contains(email.Trim()))
                                                         .Where(a => !role.HasValue || role == 0 || a.RoleId == role.Value);                                                     

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            query = query.Skip((currentPage - 1) * currentSize)
                         .Take(currentSize);

            var accounts = await query.ToListAsync();

            if (accounts == null || accounts.Count == 0)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản phù hợp", id?.ToString() ?? "No filter");
            }

            return accounts.Select(account => new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullNme = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt
            }).ToList();
        }

        public async Task DeleteAccountAsync(int accountId)
        {
            var account = _unitOfWork.Repository<Account>().Find(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Không tìm thấy tài khoản.");
            }

            _unitOfWork.Repository<Account>().Delete(account);
            await _unitOfWork.CommitAsync();
        }

        public async Task<AccountResponse> UpdateAccount(int id, AccountUpdateRequest accountRequest)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
            }
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == accountRequest.Email);

            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", accountRequest.Email.ToString());
            }

            var phoneNumberExisted = _unitOfWork.Repository<Account>().Find(x => x.PhoneNumber == accountRequest.PhoneNumber);

            if (phoneNumberExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "so dien thaoi đã tồn tại", accountRequest.PhoneNumber.ToString());
            }
            // Chỉ cập nhật Email, Password, PhoneNumber
            if (!string.IsNullOrWhiteSpace(accountRequest.Email))
                account.Email = accountRequest.Email;

            if (!string.IsNullOrWhiteSpace(accountRequest.Pasword))
            {
                CreatePasswordHash(accountRequest.Pasword, out string passwordHash);
                account.PasswordHash = passwordHash;
            }

            if (!string.IsNullOrWhiteSpace(accountRequest.PhoneNumber))
                account.PhoneNumber = accountRequest.PhoneNumber;

            account.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Account>().Update(account, id);
            await _unitOfWork.CommitAsync();

            // Lấy lại dữ liệu đã cập nhật
            account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                FullNme = account.FullName,
                Pasword = account.PasswordHash,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,

            };
        }

        

    }
}
