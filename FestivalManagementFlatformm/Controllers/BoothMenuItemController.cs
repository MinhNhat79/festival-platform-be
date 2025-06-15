using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/boothmenuitems")]
    public class BoothMenuItemController : Controller
    {
        private readonly IBoothMenuItemService _boothmenuitemservice;

        public BoothMenuItemController(IBoothMenuItemService boothmenuitemservice)
        {
            _boothmenuitemservice = boothmenuitemservice;
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BoothMenuItemCreateRequest request)
        {
            var result = await _boothmenuitemservice.CreateBoothMenuItemAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(int id, decimal? customPrice, int? quantityLimit, string? status)
       => Ok(await _boothmenuitemservice.UpdateBoothMenuItemAsync(id, customPrice, quantityLimit, status));

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? boothMenuItemId, int? boothId, int? menuItemId, string? status, int? pageNumber, int? pageSize)
            => Ok(await _boothmenuitemservice.SearchBoothMenuItemsAsync(boothMenuItemId, boothId, menuItemId, status, pageNumber, pageSize));

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _boothmenuitemservice.DeleteBoothMenuItemAsync(id);
            return NoContent();
        }
    }
}
