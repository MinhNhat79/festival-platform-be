using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class IngredientService : IIngredientService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public IngredientService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Ingredient> CreateIngredientAsync(IngredientCreateRequest request)
        {
      
            var supplier = await _unitOfWork.Repository<Supplier>().FindAsync(s => s.SupplierId == request.SupplierId);
            if (supplier == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Supplier", request.SupplierId.ToString());
            }

            var ingredient = new Ingredient
            {
                SupplierId = request.SupplierId,
                IngredientName = request.IngredientName.Trim(),
                Description = request.Description?.Trim(),
                Unit = request.Unit.Trim(),
                PricePerUnit = request.PricePerUnit,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Ingredient>().InsertAsync(ingredient);
            await _unitOfWork.CommitAsync();

            return ingredient;
        }
        public async Task<Ingredient> UpdateIngredientAsync(int id, int? supplierId, string? ingredientName,
    string? description, string? unit, decimal? pricePerUnit)
        {
            var ingredient = await _unitOfWork.Repository<Ingredient>().GetAll()
                .FirstOrDefaultAsync(i => i.IngredientId == id);

            if (ingredient == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy nguyên liệu", id.ToString());
            }

            if (supplierId.HasValue)
            {
                var supplier = await _unitOfWork.Repository<Supplier>().FindAsync(s => s.SupplierId == supplierId.Value);
                if (supplier == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Supplier", supplierId.ToString());
                }
                ingredient.SupplierId = supplierId.Value;
            }

            if (!string.IsNullOrWhiteSpace(ingredientName)) ingredient.IngredientName = ingredientName.Trim();
            if (!string.IsNullOrWhiteSpace(description)) ingredient.Description = description.Trim();
            if (!string.IsNullOrWhiteSpace(unit)) ingredient.Unit = unit.Trim();
            if (pricePerUnit.HasValue) ingredient.PricePerUnit = pricePerUnit.Value;

            ingredient.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return ingredient;
        }
        public async Task<List<Ingredient>> SearchIngredientsAsync(
        int? ingredientId, int? supplierId, string? ingredientName,
        string? unit, decimal? minPrice, decimal? maxPrice,
        int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Ingredient>().GetAll()
                .Where(i => !ingredientId.HasValue || i.IngredientId == ingredientId)
                .Where(i => !supplierId.HasValue || i.SupplierId == supplierId)
                .Where(i => string.IsNullOrWhiteSpace(ingredientName) || i.IngredientName.Contains(ingredientName.Trim()))
                .Where(i => string.IsNullOrWhiteSpace(unit) || i.Unit == unit.Trim())
                .Where(i => !minPrice.HasValue || i.PricePerUnit >= minPrice.Value)
                .Where(i => !maxPrice.HasValue || i.PricePerUnit <= maxPrice.Value);

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

         

            return result;
        }

        public async Task DeleteIngredientAsync(int id)
        {
            var item = await _unitOfWork.Repository<Ingredient>().GetAll()
                .FirstOrDefaultAsync(i => i.IngredientId == id);

            if (item == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy nguyên liệu", id.ToString());
            }

            _unitOfWork.Repository<Ingredient>().Delete(item);
            await _unitOfWork.CommitAsync();
        }

    }
}
