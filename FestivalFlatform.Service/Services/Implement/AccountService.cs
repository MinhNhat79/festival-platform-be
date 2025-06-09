using System;
using System.Collections.Generic;
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
        public void CreatePasswordHash(string password, out string passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                passwordHash = Convert.ToBase64String(hash);
            }
        }
        //Create Account Student by SchoolManager
        public async Task<AccountResponse> RegisterStudentAccountBySchoolManager(RegisterRequest request) {
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);

            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", request.Email.ToString());
            }

            CreatePasswordHash(request.Password, out string passwordHash);

            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = 3, // Đảm bảo RoleId là role học sinh
                PasswordHash = request.Password, // Nếu cần thì băm password trước              
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {           
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullNme = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.UpdatedAt
            };

        }
    }
}
