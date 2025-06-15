using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IFestivalMenuService
    {

        Task<FestivalMenu> CreateFestivalMenuAsync(FestivalMenuCreateRequest request);
        Task<FestivalMenu> UpdateFestivalMenuAsync(int menuId, string? menuName, string? description);
        Task<bool> DeleteFestivalMenuAsync(int menuId);
        Task<List<FestivalMenu>> SearchFestivalMenusAsync(
        int? menuId, int? festivalId, string? menuName, int? pageNumber, int? pageSize);
    }
}
