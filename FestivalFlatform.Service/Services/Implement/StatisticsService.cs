using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Response;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StatisticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private (DateTime? startUtc, DateTime? endUtc) NormalizeRange(string? range, DateTime? startDate, DateTime? endDate)
        {
            if (!string.IsNullOrWhiteSpace(range))
            {
                var nowUtc = DateTime.UtcNow;
                DateTime start;
                switch (range.ToLower())
                {
                    case "7d":
                    case "7days":
                    case "7":
                        start = nowUtc.Date.AddDays(-6); 
                        break;
                    case "1m":
                    case "1month":
                        start = nowUtc.Date.AddMonths(-1).AddDays(1);
                        break;
                    case "3m":
                    case "3month":
                        start = nowUtc.Date.AddMonths(-3).AddDays(1);
                        break;
                    case "1y":
                    case "1year":
                        start = nowUtc.Date.AddYears(-1).AddDays(1);
                        break;
                    default:
                        
                        return (null, null);
                }
                var end = nowUtc; // up to now
                return (start, end);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                DateTime? s = startDate?.ToUniversalTime();
                DateTime? e = endDate?.ToUniversalTime();

               
                if (s.HasValue && !e.HasValue) e = DateTime.UtcNow;
                if (!s.HasValue && e.HasValue) s = DateTime.MinValue;

                return (s, e);
            }

            
            return (null, null);
        }
        public async Task<AdminSummaryResponse> GetAdminSummaryAsync(
     string? range = null,
     DateTime? startDate = null,
     DateTime? endDate = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

           
            var totalSchools = await _unitOfWork.Repository<School>().GetAll().CountAsync();

          
            var festivalsQuery = _unitOfWork.Repository<Festival>().GetAll();
            var festivalsOngoing = await festivalsQuery
                .Where(f => f.Status != null && f.Status.ToLower() == "ongoing")
                .CountAsync();

            
            var ordersQuery = _unitOfWork.Repository<Order>().GetAll()
                .Where(o => o.Status != null && o.Status.ToLower() == "completed");

            if (startUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate >= startUtc.Value);
            if (endUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate <= endUtc.Value);

            var paidOrdersCount = await ordersQuery.CountAsync();
            decimal gmv = 0m;
            if (paidOrdersCount > 0)
            {
                gmv = await ordersQuery.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
            }

            decimal aov = paidOrdersCount > 0 ? (gmv / paidOrdersCount) : 0m;

           
            var boothsQuery = _unitOfWork.Repository<Booth>().GetAll();
            var boothsActive = await boothsQuery
                .Where(b => b.Status != null && b.Status.ToLower() == "active")
                .CountAsync();

            
            var usersActive = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Where(a => a.RoleId != 1)
                .CountAsync();

       
            var awh = _unitOfWork.Repository<AccountWalletHistory>().GetAll()
                .Where(h => h.Type != null && h.Type.ToLower() == "topup");

            if (startUtc.HasValue) awh = awh.Where(h => h.CreatedAt >= startUtc.Value);
            if (endUtc.HasValue) awh = awh.Where(h => h.CreatedAt <= endUtc.Value);

            var walletTopup = await awh.SumAsync(h => (decimal?)h.Amount) ?? 0m;

     
            var commQuery = _unitOfWork.Repository<FestivalCommission>()
     .GetAll()
     .AsQueryable(); 

            if (startUtc.HasValue)
                commQuery = commQuery.Where(c => c.CreatedAt >= startUtc.Value);

            if (endUtc.HasValue)
                commQuery = commQuery.Where(c => c.CreatedAt <= endUtc.Value);

            var commission = await commQuery.SumAsync(c => (decimal?)c.Amount) ?? 0m;

            return new AdminSummaryResponse
            {
                Schools = totalSchools,
                FestivalsOngoing = festivalsOngoing,
                Gmv = gmv,
                PaidOrders = paidOrdersCount,
                Aov = decimal.Round(aov, 2),
                BoothsActive = boothsActive,
                UsersActive = usersActive,
                WalletTopup = walletTopup,
                Commission = commission
            };
        }


        public async Task<List<PaymentMixResponse>> GetPaymentMixAsync(
      string? range = null,
      DateTime? startDate = null,
      DateTime? endDate = null,
      int? schoolId = null,
      int? festivalId = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

            var paymentsQuery = _unitOfWork.Repository<Payment>().GetAll()
                .Include(p => p.Order) 
                .AsQueryable();

           
            if (startUtc.HasValue) paymentsQuery = paymentsQuery.Where(p => p.PaymentDate >= startUtc.Value);
            if (endUtc.HasValue) paymentsQuery = paymentsQuery.Where(p => p.PaymentDate <= endUtc.Value);

           
            if (festivalId.HasValue)
            {
                paymentsQuery = paymentsQuery.Where(p => p.Order != null && p.Order.Booth.FestivalId == festivalId.Value);
            }

           
            if (schoolId.HasValue)
            {
                paymentsQuery = paymentsQuery.Where(p => p.Order != null && p.Order.Booth.Festival.SchoolId == schoolId.Value);
            }

            
            var grouped = await paymentsQuery
                .GroupBy(p => p.PaymentMethod.ToLower())
                .Select(g => new PaymentMixResponse
                {
                    Method = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return grouped;
        }

        public async Task<List<TopFestivalResponse>> GetTopFestivalsAsync(
     string? range = null,
     DateTime? startDate = null,
     DateTime? endDate = null,
     int limit = 5,
     int? schoolId = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);


            var festivalsQuery = _unitOfWork.Repository<Festival>().GetAll()
                .Include(f => f.School)
                .AsQueryable();

            if (schoolId.HasValue)
                festivalsQuery = festivalsQuery.Where(f => f.SchoolId == schoolId.Value);

            var festivals = await festivalsQuery.ToListAsync();

          
            var ordersQuery = _unitOfWork.Repository<Order>().GetAll()
                .Include(o => o.Booth)
                .ThenInclude(b => b.Festival)
                .AsQueryable();

            if (startUtc.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startUtc.Value);
            if (endUtc.HasValue)
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endUtc.Value);

            if (schoolId.HasValue)
                ordersQuery = ordersQuery.Where(o => o.Booth.Festival.SchoolId == schoolId.Value);

            var groupedOrders = await ordersQuery
                .GroupBy(o => o.Booth.FestivalId)
                .Select(g => new
                {
                    FestivalId = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    Orders = g.Count()
                })
                .ToListAsync();

           
            var result = festivals
                .Select(f =>
                {
                    var stats = groupedOrders.FirstOrDefault(g => g.FestivalId == f.FestivalId);
                    return new TopFestivalResponse
                    {
                        FestivalId = f.FestivalId,
                        FestivalName = f.FestivalName,
                        SchoolId = f.SchoolId,
                        SchoolName = f.School.SchoolName,
                       
                        Revenue = stats?.Revenue ?? 0,
                        Orders = stats?.Orders ?? 0
                    };
                })
                .OrderByDescending(r => r.Revenue)
                .Take(limit > 0 ? limit : 5)
                .ToList();

            return result;
        }


        public async Task<SchoolSummaryResponse> GetSchoolSummaryAsync(
      int? schoolId,
      string? range = null,
      DateTime? startDate = null,
      DateTime? endDate = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

         
            IQueryable<Festival> festivalsQuery = _unitOfWork.Repository<Festival>().GetAll();
            if (schoolId.HasValue && schoolId.Value > 0)
                festivalsQuery = festivalsQuery.Where(f => f.SchoolId == schoolId.Value);

            if (startUtc.HasValue) festivalsQuery = festivalsQuery.Where(f => f.CreatedAt >= startUtc.Value);
            if (endUtc.HasValue) festivalsQuery = festivalsQuery.Where(f => f.CreatedAt <= endUtc.Value);

            var totalFestivals = await festivalsQuery.CountAsync();
            var festivalsOngoing = await festivalsQuery
                .Where(f => f.Status != null && f.Status.ToLower() == "ongoing")
                .CountAsync();

          
            var boothsQuery = _unitOfWork.Repository<Booth>().GetAll()
                .Include(b => b.Festival)
                .AsQueryable();

            if (schoolId.HasValue && schoolId.Value > 0)
                boothsQuery = boothsQuery.Where(b => b.Festival.SchoolId == schoolId.Value);

            var totalBooths = await boothsQuery.CountAsync();
            var boothsActive = await boothsQuery
                .Where(b => b.Status != null && b.Status.ToLower() == "active")
                .CountAsync();

      
            IQueryable<StudentGroup> groupsQuery = _unitOfWork.Repository<StudentGroup>().GetAll();
            if (schoolId.HasValue && schoolId.Value > 0)
                groupsQuery = groupsQuery.Where(g => g.SchoolId == schoolId.Value);
            var totalGroups = await groupsQuery.CountAsync();

            var membersQuery = _unitOfWork.Repository<GroupMember>().GetAll()
                .Include(m => m.StudentGroup)
                .AsQueryable();
            if (schoolId.HasValue && schoolId.Value > 0)
                membersQuery = membersQuery.Where(m => m.StudentGroup.SchoolId == schoolId.Value);
            var totalMembers = await membersQuery.CountAsync();

  
            var ordersQuery = _unitOfWork.Repository<Order>().GetAll()
                .Include(o => o.Booth)
                    .ThenInclude(b => b.Festival)
                .Where(o => o.Status != null && o.Status.ToLower() == "completed")
                .AsQueryable();

            if (schoolId.HasValue && schoolId.Value > 0)
                ordersQuery = ordersQuery.Where(o => o.Booth.Festival.SchoolId == schoolId.Value);

            if (startUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate >= startUtc.Value);
            if (endUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate <= endUtc.Value);

            var paidOrders = await ordersQuery.CountAsync();
            var gmv = await ordersQuery.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
            var aov = paidOrders > 0 ? gmv / paidOrders : 0m;

            return new SchoolSummaryResponse
            {
                Festivals = totalFestivals,
                FestivalsOngoing = festivalsOngoing,
                Booths = totalBooths,
                BoothsActive = boothsActive,
                Groups = totalGroups,
                Members = totalMembers,
                Gmv = gmv,
                PaidOrders = paidOrders,
                Aov = decimal.Round(aov, 2)
            };
        }


        public async Task<List<MenuMixResponse>> GetMenuMixAsync(
     int? schoolId = null,
     int? festivalId = null,
     string? range = null,
     DateTime? startDate = null,
     DateTime? endDate = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

            var query = from mi in _unitOfWork.Repository<MenuItem>().GetAll()
                        join fm in _unitOfWork.Repository<FestivalMenu>().GetAll()
                            on mi.MenuId equals fm.MenuId
                        join f in _unitOfWork.Repository<Festival>().GetAll()
                            on fm.FestivalId equals f.FestivalId
                        select new { mi, f };

         
            if (festivalId.HasValue && festivalId.Value > 0)
            {
                query = query.Where(x => x.f.FestivalId == festivalId.Value);
            }
            else if (schoolId.HasValue && schoolId.Value > 0)
            {
                query = query.Where(x => x.f.SchoolId == schoolId.Value);
            }

       
            if (startUtc.HasValue)
                query = query.Where(x => x.mi.CreatedAt >= startUtc.Value);
            if (endUtc.HasValue)
                query = query.Where(x => x.mi.CreatedAt <= endUtc.Value);

            var grouped = await query
                .GroupBy(x => x.mi.ItemType.ToLower())
                .Select(g => new MenuMixResponse
                {
                    Type = g.Key,
                    Value = g.Count()
                })
                .ToListAsync();

            return grouped;
        }




        public async Task<List<FestivalPerformanceResponse>> GetFestivalPerformanceAsync(
    int? schoolId,
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

           
            var festivalsQuery = _unitOfWork.Repository<Festival>().GetAll()
                .Where(f => f.SchoolId == schoolId);

            if (startUtc.HasValue)
                festivalsQuery = festivalsQuery.Where(f => f.StartDate >= startUtc.Value);
            if (endUtc.HasValue)
                festivalsQuery = festivalsQuery.Where(f => f.EndDate <= endUtc.Value);

            var festivals = await festivalsQuery
                .Select(f => new FestivalPerformanceResponse
                {
                    FestivalId = f.FestivalId,
                    FestivalName = f.FestivalName,
                    Revenue = f.Booths
                        .SelectMany(b => b.Orders)
                        .Sum(o => (decimal?)o.TotalAmount) ?? 0,
                    Booths = f.Booths.Count(),
                    Orders = f.Booths
                        .SelectMany(b => b.Orders)
                        .Count()
                })
                .ToListAsync();

            return festivals;
        }

        public async Task<List<BoothFunnelResponse>> GetBoothFunnelAsync(
    int? schoolId = null,
    int? festivalId = null,
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

            var query = _unitOfWork.Repository<Booth>().GetAll()
                .Include(b => b.Festival)
                .AsQueryable();

          
            if (festivalId.HasValue && festivalId.Value > 0)
            {
                query = query.Where(b => b.FestivalId == festivalId.Value);
            }
            else if (schoolId.HasValue && schoolId.Value > 0)
            {
                query = query.Where(b => b.Festival.SchoolId == schoolId.Value);
            }

          
            if (startUtc.HasValue)
                query = query.Where(b => b.ApprovalDate >= startUtc.Value);
            if (endUtc.HasValue)
                query = query.Where(b => b.ApprovalDate <= endUtc.Value);

            var result = await query
                .GroupBy(b => b.Status.ToLower())
                .Select(g => new BoothFunnelResponse
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result;
        }



        public async Task<RevenueSeriesResponse> GetRevenueSeriesAsync(
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string granularity = "day",
    int? schoolId = null,
    int? festivalId = null)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

            var ordersQuery = _unitOfWork.Repository<Order>().GetAll()
                .Include(o => o.Booth)
                    .ThenInclude(b => b.Festival)
                .Where(o => o.Status.ToLower() == "completed");

            
            if (startUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate >= startUtc.Value);
            if (endUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate <= endUtc.Value);

       
            if (festivalId.HasValue && festivalId.Value > 0)
            {
                ordersQuery = ordersQuery.Where(o => o.Booth.FestivalId == festivalId.Value);
            }
            else if (schoolId.HasValue && schoolId.Value > 0)
            {
                ordersQuery = ordersQuery.Where(o => o.Booth.Festival.SchoolId == schoolId.Value);
            }

            List<(DateTime Date, decimal Revenue, int Orders)> seriesData;

            switch (granularity.ToLower())
            {
                case "month":
                    seriesData = (await ordersQuery
                        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                        .Select(g => new
                        {
                            g.Key.Year,
                            g.Key.Month,
                            Revenue = g.Sum(o => o.TotalAmount),
                            Orders = g.Count()
                        })
                        .ToListAsync())
                        .AsEnumerable()
                        .Select(g => (
                            new DateTime(g.Year, g.Month, 1),
                            g.Revenue,
                            g.Orders))
                        .OrderBy(x => x.Item1)
                        .ToList();
                    break;

                case "year":
                    seriesData = (await ordersQuery
                        .GroupBy(o => o.OrderDate.Year)
                        .Select(g => new
                        {
                            Year = g.Key,
                            Revenue = g.Sum(o => o.TotalAmount),
                            Orders = g.Count()
                        })
                        .ToListAsync())
                        .AsEnumerable()
                        .Select(g => (
                            new DateTime(g.Year, 1, 1),
                            g.Revenue,
                            g.Orders))
                        .OrderBy(x => x.Item1)
                        .ToList();
                    break;

                default: // day
                    seriesData = (await ordersQuery
                        .GroupBy(o => o.OrderDate.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            Revenue = g.Sum(o => o.TotalAmount),
                            Orders = g.Count()
                        })
                        .ToListAsync())
                        .AsEnumerable()
                        .Select(g => (
                            g.Date,
                            g.Revenue,
                            g.Orders))
                        .OrderBy(x => x.Date)
                        .ToList();
                    break;
            }

            var response = new RevenueSeriesResponse
            {
                Series = seriesData.Select(x => new RevenueSeriesPoint
                {
                    Label = granularity.ToLower() switch
                    {
                        "month" => x.Date.ToString("MM/yyyy"),
                        "year" => x.Date.ToString("yyyy"),
                        _ => x.Date.ToString("dd/MM")
                    },
                    Date = x.Date,
                    Revenue = x.Revenue,
                    Orders = x.Orders
                }).ToList(),
                Totals = new RevenueSeriesTotals
                {
                    Revenue = seriesData.Sum(x => x.Revenue),
                    Orders = seriesData.Sum(x => x.Orders)
                }
            };

            return response;
        }



        public async Task<TopBoothsResponse> GetTopBoothsAsync(
    string? range = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    int? schoolId = null,
    int limit = 5)
        {
            var (startUtc, endUtc) = NormalizeRange(range, startDate, endDate);

            var ordersQuery = _unitOfWork.Repository<Order>().GetAll()
                .Include(o => o.Booth)
                    .ThenInclude(b => b.Festival)
                .Where(o => o.Status.ToLower() == "completed");

           
            if (startUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate >= startUtc.Value);
            if (endUtc.HasValue) ordersQuery = ordersQuery.Where(o => o.OrderDate <= endUtc.Value);

            
            if (schoolId.HasValue && schoolId.Value > 0)
                ordersQuery = ordersQuery.Where(o => o.Booth.Festival.SchoolId == schoolId.Value);

            var grouped = await ordersQuery
                .GroupBy(o => new
                {
                    o.BoothId,
                    o.Booth.BoothName,
                    o.Booth.FestivalId,
                    o.Booth.Festival.FestivalName
                })
                .Select(g => new TopBoothDto
                {
                    BoothId = g.Key.BoothId,
                    BoothName = g.Key.BoothName,
                    FestivalId = g.Key.FestivalId,
                    FestivalName = g.Key.FestivalName,
                    Revenue = g.Sum(o => o.TotalAmount),
                    Orders = g.Count()
                })
                .OrderByDescending(x => x.Revenue)
                .Take(limit > 0 ? limit : 5)
                .ToListAsync();

            return new TopBoothsResponse
            {
                Data = grouped
            };
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(
    int? schoolId = null,
    int? festivalId = null,
    int limit = 5)
        {
            var query = _unitOfWork.Repository<Order>().GetAll()
                .Include(o => o.Booth)
                    .ThenInclude(b => b.Festival)
                        .ThenInclude(f => f.School)
                .Include(o => o.Account)
                .AsQueryable();

          
            if (schoolId.HasValue && schoolId.Value > 0)
                query = query.Where(o => o.Booth.Festival.SchoolId == schoolId.Value);

         
            if (festivalId.HasValue && festivalId.Value > 0)
                query = query.Where(o => o.Booth.FestivalId == festivalId.Value);

            var recentOrders = await query
                .OrderByDescending(o => o.OrderDate)
                .Take(limit > 0 ? limit : 10)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.OrderId,
                    SchoolName = o.Booth.Festival.School.SchoolName,
                    FestivalName = o.Booth.Festival.FestivalName,
                    BoothName = o.Booth.BoothName,
                    UserFullName = o.Account.FullName,
                    Amount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.OrderDate
                })
                .ToListAsync();

            return recentOrders;
        }

        public async Task<List<AlertDto>> GetAlertsAsync()
        {
            var alerts = new List<AlertDto>();

            // Booth pending
            var boothPending = await _unitOfWork.Repository<Booth>().GetAll()
                .Where(b => b.Status.ToLower() == "pending")
                .GroupBy(b => b.FestivalId)
                .Select(g => new AlertDto
                {
                    Type = "booth_pending",
                    Count = g.Count(),
                    Message = g.Count() + " gian hàng chờ duyệt",
                    FestivalId = g.Key
                })
                .ToListAsync();

            alerts.AddRange(boothPending);

            // Festival pending (bảng FestivalSchool)
            var festivalPending = await _unitOfWork.Repository<FestivalSchool>().GetAll()
                .Where(fs => fs.Status.ToLower() == "pending")
                .GroupBy(fs => fs.FestivalId)
                .Select(g => new AlertDto
                {
                    Type = "festival_pending",
                    Count = g.Count(),
                    Message = g.Count() + " lễ hội đang chờ duyệt",
                    FestivalId = g.Key
                })
                .ToListAsync();

            alerts.AddRange(festivalPending);

            return alerts;
        }

    }

}
