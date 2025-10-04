using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _iloginService;

        public LoginController(ILoginService loginService)
        {
            _iloginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginnRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request."
                });
            }

            try
            {
                var result = await _iloginService.AuthenticateAsync(request.Email, request.Password);

                if (result != null && result.Success)
                {
                    return Ok(result);
                }

                return Unauthorized(new
                {
                    success = false,
                    message = result?.Message ?? "Invalid credentials."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }
        [HttpPost("google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var response = await _iloginService.LoginWithGoogleAsync(request.Token);
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        

        public class GoogleLoginRequest
        {
            public string Token { get; set; } = null!;
        }
    }
}
