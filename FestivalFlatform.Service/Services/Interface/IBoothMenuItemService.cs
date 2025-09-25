using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public  interface IBoothMenuItemService
    {
        Task<BoothMenuItem> CreateBoothMenuItemAsync(BoothMenuItemCreateRequest request);
        Task DeleteBoothMenuItemAsync(int id);
        Task<List<BoothMenuItem>> SearchBoothMenuItemsAsync(
        int? boothMenuItemId, int? boothId, int? menuItemId, string? status, int? pageNumber, int? pageSize);
        Task<BoothMenuItem> UpdateBoothMenuItemAsync(int id, decimal? customPrice, int? quantityLimit, int? remainingQuantity, string? status);
    }
}
