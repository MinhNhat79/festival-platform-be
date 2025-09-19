using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IFestivalCommissionService
    {
        Task<FestivalCommission> CreateAsync(FestivalCommissionCreateRequest request);
        Task<FestivalCommission> UpdateAsync(int commissionId, decimal? amount, double? commissionRate);
        Task<List<FestivalCommission>> SearchFestivalCommissionsAsync(
        int? commissionId,
        int? festivalId,
        int? pageNumber,
        int? pageSize);
        Task DeleteAsync(int commissionId);
    }
}
