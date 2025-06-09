using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Response;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FestivalFlatform.Service.Services.Implement
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;



        public LoginService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<LoginService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;

        }


        public async Task<LoginResponse> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Email và mật khẩu không được để trống."
                };
            }

            try
            {
                var account = await _unitOfWork.Repository<Account>()
                    .GetAll()
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.Email == email);

                if (account == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Sai email hoặc mật khẩu."
                    };
                }

                // So sánh mật khẩu trực tiếp (hoặc dùng hàm verify nếu hash)
                if (account.PasswordHash != password)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Sai email hoặc mật khẩu."
                    };
                }

                var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, account.Email),
    new Claim(ClaimTypes.Role, account.Role?.RoleName ?? ""), // đảm bảo null-safe
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // 👈 thêm dòng này
};


                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtAuth:Key"]));
                var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtAuth:Issuer"],
                    audience: _configuration["JwtAuth:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return new LoginResponse
                {
                    Success = true,
                    AccessToken = tokenString,
                    Id = account.AccountId,
                    UserName = account.Email,
                    FullName = account.FullName,
                    Role = account.Role?.RoleName ?? "User"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình đăng nhập.");
                return new LoginResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi đăng nhập."
                };
            }
        }

    }


}
