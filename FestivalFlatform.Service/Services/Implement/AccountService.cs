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
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

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
            // Kiểm tra email
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);
            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", request.Email);
            }

            // Kiểm tra số điện thoại
            var phoneNumberExisted = _unitOfWork.Repository<Account>().Find(x => x.PhoneNumber == request.PhoneNumber);
            if (phoneNumberExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Số điện thoại đã tồn tại", request.PhoneNumber);
            }

            // Tạo mật khẩu hash
            CreatePasswordHash(request.Password, out string passwordHash);

            // Tạo account mới
            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync(); // Commit để có AccountId

            // ✅ Tạo wallet cho account vừa tạo
            var wallet = new Wallet
            {
                AccountId = account.AccountId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullName = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
                // Nếu muốn trả về WalletId luôn:
                WalletId = wallet.WalletId
            };
        }

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
                throw new CrudException(HttpStatusCode.Conflict, "so dien thoai đã tồn tại");
            }
            var studentRole = await _unitOfWork.Repository<Role>()
            .GetAll()
            .FirstOrDefaultAsync(r => r.RoleName.ToLower() == "Student");

            if (studentRole == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy role 'student'");
            }
            CreatePasswordHash(request.Password, out string passwordHash);

            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = studentRole.RoleId,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullName = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
            };
        }


        public async Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, int? role, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Account>().GetAll()

                                                         .Where(a => !id.HasValue || id == 0 || a.AccountId == id.Value)

                                                         .Where(a => string.IsNullOrWhiteSpace(phone) || a.PhoneNumber.Contains(phone.Trim()))
                                                         .Where(a => string.IsNullOrWhiteSpace(email) || a.Email.Contains(email.Trim()))
                                                         .Where(a => !role.HasValue || role == 0 || a.RoleId == role.Value);

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize)
            //             .Take(currentSize);

            var accounts = await query.ToListAsync();


            return accounts.Select(account => new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullName = account.FullName,
                PhoneNumber = account.PhoneNumber,
                AvatarUrl = account.AvatarUrl,
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
                throw new CrudException(HttpStatusCode.Conflict, "so dien thoai đã tồn tại", accountRequest.PhoneNumber.ToString());
            }


            if (!string.IsNullOrWhiteSpace(accountRequest.Email))
                account.FullName = accountRequest.FullName;

            if (!string.IsNullOrWhiteSpace(accountRequest.Email))
                account.Email = accountRequest.Email;

            if (!string.IsNullOrWhiteSpace(accountRequest.Pasword))
            {
                CreatePasswordHash(accountRequest.Pasword, out string passwordHash);
                account.PasswordHash = passwordHash;
            }

            if (!string.IsNullOrWhiteSpace(accountRequest.PhoneNumber))
                account.PhoneNumber = accountRequest.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(accountRequest.AvatarUrl))
                account.AvatarUrl = accountRequest.AvatarUrl;


            account.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Account>().Update(account, id);
            await _unitOfWork.CommitAsync();


            account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                FullName = account.FullName,
                Pasword = account.PasswordHash,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,

            };
        }




        public async Task<(List<AccountResponse> successAccounts, List<string> errors)> ImportAccountsFromExcelAsync(int schoolId, Stream excelStream)
        {
            List<AccountResponse> createdAccounts = new();
            List<string> errors = new();

            IWorkbook workbook = new XSSFWorkbook(excelStream);
            ISheet sheet = workbook.GetSheetAt(0);

            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++) // Bỏ header dòng 0
            {
                IRow? row = sheet.GetRow(rowIndex);
                if (row == null) continue;

                string? fullName = row.GetCell(0)?.ToString()?.Trim();
                string? phoneNumber = row.GetCell(1)?.ToString()?.Trim();
                string? email = row.GetCell(2)?.ToString()?.Trim();
                string? password = row.GetCell(3)?.ToString()?.Trim();
                string? roleName = row.GetCell(4)?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    errors.Add($"Dòng {rowIndex + 1}: Email hoặc Password bị trống");
                    continue;
                }

                // Kiểm tra email hoặc phone tồn tại
                var emailExists = _unitOfWork.Repository<Account>().Find(a => a.Email == email);
                if (emailExists != null)
                {
                    errors.Add($"Dòng {rowIndex + 1}: Email '{email}' đã tồn tại");
                    continue;
                }
                var phoneExists = _unitOfWork.Repository<Account>().Find(a => a.PhoneNumber == phoneNumber);
                if (phoneExists != null)
                {
                    errors.Add($"Dòng {rowIndex + 1}: Số điện thoại '{phoneNumber}' đã tồn tại");
                    continue;
                }

                // Lấy RoleId theo roleName
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    errors.Add($"Dòng {rowIndex + 1}: Role không được để trống");
                    continue;
                }

                string roleNameLower = roleName.ToLower();

                var role = await _unitOfWork.Repository<Role>()
                    .GetAll()
                    .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleNameLower);

                if (role == null)
                {
                    errors.Add($"Dòng {rowIndex + 1}: Role '{roleName}' không tồn tại");
                    continue;
                }

                CreatePasswordHash(password, out string passwordHash);

                var account = new Account
                {
                    FullName = fullName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RoleId = role.RoleId,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Account>().InsertAsync(account);
                await _unitOfWork.CommitAsync();

                // ✅ Tạo Wallet ngay sau khi tạo Account
                var wallet = new Wallet
                {
                    AccountId = account.AccountId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
                await _unitOfWork.CommitAsync();

                createdAccounts.Add(new AccountResponse
                {
                    Id = account.AccountId,
                    Email = account.Email,
                    FullName = account.FullName,
                    PhoneNumber = account.PhoneNumber,
                    RoleId = account.RoleId,
                    CreatedAt = account.CreatedAt,
                    WalletId = wallet.WalletId,
                    Balance = wallet.Balance
                });

                // Tạo SchoolAccountRelation
                var relation = new SchoolAccountRelation
                {
                    SchoolId = schoolId,
                    AccountId = account.AccountId,
                    RelationType = role.RoleName
                };
                await _unitOfWork.Repository<SchoolAccountRelation>().InsertAsync(relation);
                await _unitOfWork.CommitAsync();
            }

            return (createdAccounts, errors);
        }

        // Nếu muốn lấy relationType đã đọc lúc tạo account, bạn có thể lưu lại map accountId -> relationType ở trên và dùng
        private async Task<string> GetRelationTypeForAccountAsync(int accountId)
        {
            // TODO: tùy bạn lưu relationType như nào, hoặc truyền param vào
            return "student"; // placeholder
        }
        public async Task<bool> UpdatePasswordAsync(int accountId, string oldPassword, string newPassword)
        {
            {
                // Lấy account từ DB
                var account = await _unitOfWork.Repository<Account>()
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);

                if (account == null)
                    throw new Exception("❌ Tài khoản không tồn tại");

                // Kiểm tra mật khẩu cũ
                bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(oldPassword, account.PasswordHash);
                if (!isOldPasswordCorrect)
                    throw new Exception("❌ Mật khẩu cũ không đúng");

                // Hash mật khẩu mới
                string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // Cập nhật vào DB
                account.PasswordHash = hashedNewPassword;
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
        }


    }
}
