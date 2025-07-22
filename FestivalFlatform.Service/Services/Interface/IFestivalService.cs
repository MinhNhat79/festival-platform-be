using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface  IFestivalService
    {
        Task<Festival> CreateFestivalAsync(FestivalCreateRequest request);
        Task<Festival> UpdateFestivalAsync(int festivalId, int? maxFoodBooths, int? maxBeverageBooths, int? registeredFoodBooths, int? registeredBeverageBooths,string? cancelReason, string? status);
        Task<List<Festival>> SearchFestivalsAsync(int? festivalId, int? schoolId, string? status,
        DateTime? startDate, DateTime? endDate, DateTime? registrationStartDate, DateTime? registrationEndDate,
        int? pageNumber, int? pageSize);
        Task DeleteFestivalAsync(int festivalId);
    }
}
