using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Response
{
    public class SchoolSummaryResponse
    {
        public int Festivals { get; set; }
        public int FestivalsOngoing { get; set; }
        public int Booths { get; set; }
        public int BoothsActive { get; set; }
        public int Groups { get; set; }
        public int Members { get; set; }
        public decimal Gmv { get; set; }
        public int PaidOrders { get; set; }
        public decimal Aov { get; set; }
    }

    public class MenuMixResponse
    {
        public string Type { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class FestivalPerformanceResponse
    {
        public int FestivalId { get; set; }
        public string FestivalName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Booths { get; set; }
        public int Orders { get; set; }
    }
    public class BoothFunnelResponse
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class RevenueSeriesPoint
    {
        public string Label { get; set; } = string.Empty; // hiển thị (01/09, 09/2025,...)
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class RevenueSeriesResponse
    {
        public List<RevenueSeriesPoint> Series { get; set; } = new();
        public RevenueSeriesTotals Totals { get; set; } = new();
    }

    public class RevenueSeriesTotals
    {
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class TopBoothsResponse
    {
        public List<TopBoothDto> Data { get; set; } = new();
    }

    public class TopBoothDto
    {
        public int BoothId { get; set; }
        public string BoothName { get; set; } = null!;
        public int FestivalId { get; set; }
        public string FestivalName { get; set; } = null!;
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public string SchoolName { get; set; } = null!;
        public string FestivalName { get; set; } = null!;
        public string BoothName { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
