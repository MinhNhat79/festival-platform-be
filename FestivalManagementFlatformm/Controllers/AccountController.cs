using System.Text.RegularExpressions;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize(Roles = Roles.SchoolManager, AuthenticationSchemes = "Bearer")]
        [HttpPost("api/accounts/register-student-account")]
        public async Task<ActionResult<AccountResponse>> RegisterStudentAccountBySchool([FromBody] RegisterRequest request)
        {
            try
            {
                // Kiểm tra định dạng tên
                var regexName = new Regex("^[a-zA-Z ]+$");
                if (!regexName.IsMatch(request.FullName))
                {
                    return BadRequest("Tên không hợp lệ");
                }

                // Kiểm tra định dạng số điện thoại
                var regexPhone = new Regex("^[0-9]+$");
                if (!regexPhone.IsMatch(request.PhoneNumber))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }

                if (request.PhoneNumber.Length < 9 || request.PhoneNumber.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                }

                // Kiểm tra định dạng email (chỉ gmail)
                var regexEmail = new Regex(@"^\w+@gmail\.com$");
                if (!regexEmail.IsMatch(request.Email))
                {
                    return BadRequest("Email phải có định dạng @gmail.com");
                }

                // Gọi service
                var result = await _accountService.RegisterStudentAccountBySchoolManager(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đăng ký thất bại: {ex.Message}");
            }
        }
        

        
    }
}
