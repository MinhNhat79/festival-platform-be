using System.Net.Http;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/logins")]
    public class LoginController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoginService _iloginService;


        public LoginController(ILoginService loginService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _iloginService = loginService;



        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginnRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid request.");
            }

            var result = await _iloginService.AuthenticateAsync(request.Email, request.Password);

            dynamic dynamicResult = result;

            if (dynamicResult != null)
            {
                if (dynamicResult.Success)
                {
                    return Ok(new
                    {
                        AccessToken = dynamicResult.AccessToken,
                        id = result.Id,
                        UserName = result.UserName,
                        FullName = result.FullName,
                        Role = result.Role,
                    });
                }
                else
                {
                    return Unauthorized(dynamicResult.Message);
                }
            }

            return BadRequest("Invalid result.");
        }
    }
}
