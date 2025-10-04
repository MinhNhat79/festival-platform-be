using System.Net;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
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
        public async Task<IActionResult> CreateBooth([FromBody] BoothCreateRequest request)
        {
            try
            {
                var createdBooth = await _boothService.CreateBoothAsync(request);
                return Ok(createdBooth);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Tạo gian hàng thất bại", detail = ex.Message });
            }
        }

        [HttpPut("approve")]
        public async Task<IActionResult> ApproveBooth(int id, [FromBody] BoothApproveRequest request)
        {
            try
            {
                var updatedBooth = await _boothService.UpdateBoothApproveAsync(id, request.ApprovalDate, request.PointsBalance);
                if (updatedBooth == null)
                    return NotFound(new { success = false, message = "Gian hàng không tồn tại" });

                return Ok(updatedBooth);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Phê duyệt thất bại", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooths(
            [FromQuery] int? boothId,
            [FromQuery] int? groupId,
            [FromQuery] int? festivalId,
            [FromQuery] int? locationId,
            [FromQuery] string? boothType,
            [FromQuery] string? status,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            try
            {
                var booths = await _boothService.GetBooths(boothId, groupId, festivalId, locationId, boothType, status, pageNumber, pageSize);
                return Ok(booths);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lấy danh sách gian hàng thất bại", detail = ex.Message });
            }
        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectBooth(int boothId, string? rejectReason)
        {
            try
            {
                await _boothService.UpdateBoothStatusToRejected(boothId, rejectReason);
                return Ok(new { success = true, message = "Booth status updated to Rejected" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Gian hàng không tồn tại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Cập nhật trạng thái thất bại", detail = ex.Message });
            }
        }

        [HttpPut("activate")]
        public async Task<IActionResult> ActivateBooth(int boothId)
        {
            try
            {
                await _boothService.UpdateBoothStatusToActive(boothId);
                return Ok(new { success = true, message = "Booth status updated to Active" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Gian hàng không tồn tại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Cập nhật trạng thái thất bại", detail = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBooth(int boothId)
        {
            try
            {
                await _boothService.DeleteBoothAsync(boothId);
                return Ok(new { success = true, message = "Xóa gian hàng thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Gian hàng không tồn tại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Xóa gian hàng thất bại", detail = ex.Message });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateBooth(int boothId, [FromBody] BoothUpdateRequest request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Request body không hợp lệ" });

            try
            {
                var booth = await _boothService.UpdateBoothAsync(boothId, request);
                if (booth == null)
                    return NotFound(new { success = false, message = "Không tìm thấy booth" });

                return Ok(new { success = true, message = "Cập nhật booth thành công", data = booth });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
        [HttpPost("withdraw-revenue")]
        public async Task<IActionResult> WithdrawRevenue([FromBody] WithdrawRevenueRequest request)
        {
            try
            {
                var booth = await _boothService.WithdrawBoothRevenueAsync(request.BoothId, request.AccountId);

                return Ok(new
                {
                    Success = true,
                    Message = "Chuyển doanh thu thành công",
                    Booth = booth
                });
            }
            catch (CrudException ex) 
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex) 
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        public class WithdrawRevenueRequest
        {
            public int BoothId { get; set; }
            public int AccountId { get; set; }
        }

        [HttpGet("can-withdraw-revenue")]
        public async Task<IActionResult> CanWithdrawRevenue([FromQuery] int boothId, [FromQuery] int accountId)
        {
            try
            {
                var canWithdraw = await _boothService.CanWithdrawRevenueAsync(boothId, accountId);
                return Ok(new
                {
                    Success = true,
                    CanWithdraw = canWithdraw
                });
            }
            catch (CrudException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
