using System.Drawing.Printing;
using System.Net;
using System.Text.RegularExpressions;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        public AccountController(IAccountService accountService, IEmailService emailService, IConfiguration config)
        {
            _accountService = accountService;
            _emailService = emailService;
            _config = config;
        }

        [Authorize(Roles = Roles.SchoolManager, AuthenticationSchemes = "Bearer")]
        [HttpPost("api/accounts/create-student")]
        public async Task<ActionResult<AccountResponse>> RegisterStudentAccountBySchool([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _accountService.RegisterStudentAccountBySchoolManager(request);
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Đăng ký thất bại: {ex.Message}" });
            }
        }

        [HttpPost("api/accounts/create")]
        public async Task<ActionResult<AccountResponse>> RegisterAccountBy([FromBody] RegisterRequestAll request)
        {
            try
            {
                var regexPhone = new Regex(@"^\d{9,11}$");
                if (!regexPhone.IsMatch(request.PhoneNumber))
                    return BadRequest(new { success = false, message = "Số điện thoại không hợp lệ" });

                //var regexEmail = new Regex(@"^[a-zA-Z0-9._%+-]+@gmail\.com$");
                //if (!regexEmail.IsMatch(request.Email))
                //    return BadRequest(new { success = false, message = "Email phải có định dạng @gmail.com" });

                var result = await _accountService.RegisterAccount(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Đăng ký thất bại: {ex.Message}" });
            }
        }


        //[Authorize(Roles = Roles.SchoolManager, AuthenticationSchemes = "Bearer")]
        [HttpGet("api/accounts/search")]
        public async Task<ActionResult<IEnumerable<AccountResponse>>> SearchAccountsByFields(
      [FromQuery] int? id,
      [FromQuery] string? phone,
      [FromQuery] string? email,
      [FromQuery] string? className,
      [FromQuery] int? role,
      [FromQuery] int? pageNumber,
      [FromQuery] int? pageSize)
        {
            try
            {
                var result = await _accountService.GetAccount(id, phone, email, className, role, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpDelete("api/accounts/delete")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                await _accountService.DeleteAccountAsync(id);
                return Ok(new { success = true, message = "Xóa tài khoản thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Tài khoản không tồn tại." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }



        
        [HttpPut("api/accounts/update")]
        public async Task<IActionResult> UpdateAccountAsync(int id, AccountUpdateRequest request)
        {
            try
            {
                var account = await _accountService.UpdateAccount(id, request);

                if (account != null)
                    return Ok(new { success = true, message = "🔑 Cập nhật thành công", data = account });

                return BadRequest(new { success = false, message = "❌ Cập nhật mật khẩu thất bại" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }

        [HttpPost("api/accounts/import-accounts")]
        public async Task<IActionResult> ImportAccounts([FromForm] ImportAccountsRequest request)
        {
            if (request.ExcelFile == null || request.ExcelFile.Length == 0)
                return BadRequest(new { success = false, message = "File Excel không được để trống." });

            using var stream = request.ExcelFile.OpenReadStream();

            var (createdAccounts, errors) = await _accountService.ImportAccountsFromExcelAsync(request.SchoolId, stream);

            return Ok(new
            {
                success = errors.Count == 0,
                createdAccounts,
                errors
            });
        }
        [HttpPut("api/accounts/update-password")]
        public async Task<IActionResult> UpdatePassword(int accountId, string oldPassword, string newPassword)
        {
            try
            {
                bool result = await _accountService.UpdatePasswordAsync(accountId, oldPassword, newPassword);

                if (result)
                    return Ok(new { success = true, message = "🔑 Cập nhật mật khẩu thành công" });

                return BadRequest(new { success = false, message = "❌ Cập nhật mật khẩu thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }


        [HttpPost("api/accounts/send-email")]
        public async Task<IActionResult> SendEmail([FromBody] string email)
        {
            await _accountService.SendEmailWithButtonAsync(
                email);

            return Ok(new { Message = $"Email sent to {email}" });
        }

        [HttpPost("api/accounts/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { success = false, message = "Email không hợp lệ" });

            try
            {
                var success = await _accountService.ForgotPasswordAsync(request.Email);

                if (!success)
                    return StatusCode(500, new { success = false, message = "Không thể gửi email khôi phục mật khẩu" });

                return Ok(new { success = true, message = "Mật khẩu mới đã được gửi tới email của bạn" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }


        [HttpPost("api/accounts/send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            try
            {
                var success = await _accountService.SendOtpAsync(email);
                return Ok(new { success, message = "OTP đã được gửi đến email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/accounts/confirm-otp")]
        public async Task<IActionResult> ConfirmOtp([FromBody] ConfirmOtpRequest request)
        {
            try
            {
                var success = await _accountService.ConfirmOtpAsync(request.Email, request.Otp);
                return Ok(new { success, message = "Mật khẩu mới đã được gửi đến email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("api/accounts/IsDelete")]
        public async Task<IActionResult> SoftDeleteAccount(int id)
        {
            var success = await _accountService.SoftDeleteAccountAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy tài khoản" });

            return Ok(new { message = "Đã xoá tài khoản thành công" });
        }
    }
}
