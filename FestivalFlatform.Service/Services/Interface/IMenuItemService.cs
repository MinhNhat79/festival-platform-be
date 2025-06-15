using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IMenuItemService
    {
        Task<MenuItem> CreateMenuItemAsync(MenuItemCreateRequest request);

        Task DeleteMenuItemAsync(int itemId);
        Task<List<MenuItem>> SearchMenuItemsAsync(
       int? itemId, int? menuId, string? itemName, string? itemType,
       int? pageNumber, int? pageSize);
        Task<MenuItem> UpdateMenuItemAsync(
       int itemId,
       int menuId,
       string itemName,
       string? description,
       string itemType,
       decimal basePrice);

    }

}
