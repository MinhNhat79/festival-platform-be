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
        [HttpPost("api/accounts/create-student")]
        public async Task<ActionResult<AccountResponse>> RegisterStudentAccountBySchool([FromBody] RegisterRequest request)
        {
            try
            {
               

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

        [HttpPost("api/accounts/create")]
        public async Task<ActionResult<AccountResponse>> RegisterAccountBy([FromBody] RegisterRequestAll request)
        {
            try
            {
       
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
                var result = await _accountService.RegisterAccount(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đăng ký thất bại: {ex.Message}");
            }
        }

        //[Authorize(Roles = Roles.SchoolManager, AuthenticationSchemes = "Bearer")]
        [HttpGet("api/accounts/search")]
        public async Task<ActionResult<IEnumerable<AccountResponse>>> SearchAccountsByFields([FromQuery] int? id, [FromQuery] string? phone, [FromQuery] string? email, [FromQuery] int? role,[FromQuery] int? pageNumber,[FromQuery] int? pageSize)
        {
            try
            {
                var result = await _accountService.GetAccount(id, phone, email, role, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tìm kiếm tài khoản: {ex.Message}");
            }

        }

        [HttpDelete("api/accounts/delete")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                await _accountService.DeleteAccountAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Tài khoản không tồn tại." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", detail = ex.Message });
            }
        }


        [HttpPut("api/accounts/update")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountUpdateRequest accountUpdateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedAccount = await _accountService.UpdateAccount(id, accountUpdateRequest);
                return Ok(updatedAccount);
            }
            catch (CrudException ex)
            {
                // Chỉ check theo message hoặc tạo custom logic nếu cần
                if (ex.Message.Contains("Không tìm thấy tài khoản"))
                    return NotFound(new { message = ex.Message });

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, new { message = "Lỗi server", details = ex.Message });
            }
        }

    }
}
