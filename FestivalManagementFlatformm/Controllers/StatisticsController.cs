using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }
        [HttpGet("admin/summary")]
        public async Task<IActionResult> GetSummary(
           [FromQuery] string? range,
           [FromQuery] DateTime? startDate,
           [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetAdminSummaryAsync(range, startDate, endDate);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy summary", detail = ex.Message });
            }
        }

        [HttpGet("admin/payment-mix")]
        public async Task<IActionResult> GetPaymentMix(
    [FromQuery] string? range,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] int? schoolId,
    [FromQuery] int? festivalId)
        {
            try
            {
                var result = await _statisticsService.GetPaymentMixAsync(range, startDate, endDate, schoolId, festivalId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("admin/top-festivals")]
        public async Task<IActionResult> GetTopFestivals(
            [FromQuery] string? range,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int limit,
            [FromQuery] int? schoolId)
        {
            try
            {
                var result = await _statisticsService.GetTopFestivalsAsync(range, startDate, endDate, limit, schoolId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy top-festivals", detail = ex.Message });
            }
        }

        [HttpGet("school/summary")]
        public async Task<IActionResult> GetSchoolSummary(
    [FromQuery] int schoolId,
    [FromQuery] string? range,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetSchoolSummaryAsync(schoolId, range, startDate, endDate);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy school summary", detail = ex.Message });
            }
        }


        [HttpGet("school/menu-mix")]
        public async Task<IActionResult> GetMenuMix(
    [FromQuery] int? schoolId,
    [FromQuery] int? festivalId,
    [FromQuery] string? range,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetMenuMixAsync(schoolId, festivalId, range, startDate, endDate);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy menu mix", detail = ex.Message });
            }

        }


        [HttpGet("school/festival-performance")]
        public async Task<IActionResult> GetFestivalPerformance(
    [FromQuery] int schoolId,
    [FromQuery] string? range,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetFestivalPerformanceAsync(schoolId, range, startDate, endDate);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy festival performance", detail = ex.Message });
            }
        }

        [HttpGet("school/booth-funnel")]
        public async Task<IActionResult> GetBoothFunnel(
    [FromQuery] int schoolId,
    [FromQuery] int? festivalId,
    [FromQuery] string? range,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _statisticsService.GetBoothFunnelAsync(schoolId, festivalId, range, startDate, endDate);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy booth funnel", detail = ex.Message });
            }
        }
        [HttpGet("revenue-series")]
        public async Task<IActionResult> GetRevenueSeries(
    [FromQuery] string? range,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] string granularity = "day",
    [FromQuery] int? schoolId = null,
    [FromQuery] int? festivalId = null)
        {
            try
            {
                var result = await _statisticsService.GetRevenueSeriesAsync(range, startDate, endDate, granularity, schoolId, festivalId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }



        [HttpGet("top-booths")]
        public async Task<IActionResult> GetTopBooths(
    [FromQuery] string? range,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] int? schoolId,
    [FromQuery] int limit = 5)
        {
            try
            {
                var result = await _statisticsService.GetTopBoothsAsync(range, startDate, endDate, schoolId, limit);
                return Ok(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy top booths", detail = ex.Message });
            }
        }

        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders(
    [FromQuery] int? schoolId,
    [FromQuery] int? festivalId,
    [FromQuery] int limit = 10)
        {
            try
            {
                var result = await _statisticsService.GetRecentOrdersAsync(schoolId, festivalId, limit);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy recent orders", detail = ex.Message });
            }
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            try
            {
                var result = await _statisticsService.GetAlertsAsync();
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy alerts", detail = ex.Message });
            }
        }
    }
}
