using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IIngredientService
    {
        Task<Ingredient> CreateIngredientAsync(IngredientCreateRequest request);

        Task<Ingredient> UpdateIngredientAsync(int id, int? supplierId, string? ingredientName,
        string? description, string? unit, decimal? pricePerUnit);
        Task<List<Ingredient>> SearchIngredientsAsync(
        int? ingredientId, int? supplierId, string? ingredientName,
        string? unit, decimal? minPrice, decimal? maxPrice,
        int? pageNumber, int? pageSize);
        Task DeleteIngredientAsync(int id);
    }
}
