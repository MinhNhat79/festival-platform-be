using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/booths")]
    public class BoothController : Controller
    {
        private readonly IBoothService _boothService;

        public BoothController(IBoothService boothService)
        {
            _boothService = boothService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Booth>> CreateBooth([FromBody] BoothCreateRequest request)
        {
            try
            {
                var createdBooth = await _boothService.CreateBoothAsync(request);
                return Ok(createdBooth);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Tạo gian hàng thất bại", detail = ex.Message });
            }
        }

        [HttpPut("approve")]
        public async Task<IActionResult> ApproveBooth(int id, [FromBody] BoothApproveRequest request)
        {
            var updatedBooth = await _boothService.UpdateBoothAsync(id, request.ApprovalDate, request.PointsBalance);
            if (updatedBooth == null)
                return NotFound();

            return Ok(updatedBooth);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooths([FromQuery] int? boothId,[FromQuery] int? groupId,[FromQuery] int? festivalId,[FromQuery] int? locationId,[FromQuery] string? boothType,[FromQuery] string? status,[FromQuery] int? pageNumber,[FromQuery] int? pageSize)
        {
            var booths = await _boothService.GetBooths(boothId,groupId,festivalId,locationId,boothType,status,pageNumber,pageSize);

            return Ok(booths);
        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectBooth(int boothId)
        {
            await _boothService.UpdateBoothStatusToRejected(boothId);
            return Ok(new { message = "Booth status updated to Rejected" });
        }

        [HttpPut("activate")]
        public async Task<IActionResult> ActivateBooth(int boothId)
        {
            await _boothService.UpdateBoothStatusToActive(boothId);
            return Ok(new { message = "Booth status updated to Active" });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBooth(int boothId)
        {
            await _boothService.DeleteBoothAsync(boothId);
            return Ok(new { message = "Xóa gian hàng thành công" });
        }


    }
}
