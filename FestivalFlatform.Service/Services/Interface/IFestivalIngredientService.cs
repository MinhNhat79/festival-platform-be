using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IFestivalIngredientService
    {
        Task<FestivalIngredient> CreateFestivalIngredientAsync(FestivalIngredientCreateRequest request);
        Task<FestivalIngredient> UpdateFestivalIngredientAsync(int id, int? festivalId, int? ingredientId, int? quantityAvailable, decimal? specialPrice, string? status);
        Task<List<FestivalIngredient>> SearchFestivalIngredientsAsync(
        int? festivalIngredientId, int? festivalId, int? ingredientId,
        string? status, int? pageNumber, int? pageSize);
        Task DeleteFestivalIngredientAsync(int id);
        Task UpdateFestivaIngredientStatusToRejectAsync(int id, string? rejectReason);
    }
}
