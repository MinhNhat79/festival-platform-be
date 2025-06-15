using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IMenuItemIngredientService
    {
        Task<MenuItemIngredient> CreateMenuItemIngredientAsync(CreateMenuItemIngredientRequest request);
        Task<MenuItemIngredient> UpdateMenuItemIngredientAsync(
       int itemIngredientId,
       int itemId,
       int ingredientId,
       decimal quantity,
       string unit);
        Task<List<MenuItemIngredient>> SearchMenuItemIngredientsAsync(
        int? itemIngredientId,
        int? itemId,
        int? ingredientId,
        string? unit,
        int? pageNumber,
        int? pageSize);
        Task<bool> DeleteMenuItemIngredientAsync(int itemIngredientId);
    }
}
