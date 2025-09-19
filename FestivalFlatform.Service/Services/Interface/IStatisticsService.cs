using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Service.DTOs.Response;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IStatisticsService
    {
        Task<AdminSummaryResponse> GetAdminSummaryAsync(
       string? range = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<PaymentMixResponse>> GetPaymentMixAsync(
      string? range = null,
      DateTime? startDate = null,
      DateTime? endDate = null,
      int? schoolId = null,
      int? festivalId = null);

        Task<List<TopFestivalResponse>> GetTopFestivalsAsync(
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    int limit = 5,
    int? schoolId = null);

        Task<SchoolSummaryResponse> GetSchoolSummaryAsync(
    int schoolId,
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null);

        Task<List<MenuMixResponse>> GetMenuMixAsync(
      int? schoolId,
      int? festivalId = null,
      string? range = null,
      DateTime? startDate = null,
      DateTime? endDate = null);

        Task<List<FestivalPerformanceResponse>> GetFestivalPerformanceAsync(
    int schoolId,
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null);

        Task<List<BoothFunnelResponse>> GetBoothFunnelAsync(
       int? schoolId = null,
       int? festivalId = null,
       string? range = null,
       DateTime? startDate = null,
       DateTime? endDate = null);


        Task<RevenueSeriesResponse> GetRevenueSeriesAsync(
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string granularity = "day",
    int? schoolId = null,
    int? festivalId = null);


        Task<TopBoothsResponse> GetTopBoothsAsync(
            string? range = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? schoolId = null,
            int limit = 5);

        Task<List<RecentOrderDto>> GetRecentOrdersAsync(
    int? schoolId = null,
    int? festivalId = null,
    int limit = 5);


        Task<List<AlertDto>> GetAlertsAsync();
    }
}
