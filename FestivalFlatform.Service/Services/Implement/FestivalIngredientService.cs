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
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class FestivalIngredientService : IFestivalIngredientService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public FestivalIngredientService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<FestivalIngredient> CreateFestivalIngredientAsync(FestivalIngredientCreateRequest request)
        {
            // Validate Festival
            var festival = await _unitOfWork.Repository<Festival>()
                .FindAsync(f => f.FestivalId == request.FestivalId);
            if (festival == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Festival", request.FestivalId.ToString());
            }

            // Validate Ingredient
            var ingredient = await _unitOfWork.Repository<Ingredient>()
                .FindAsync(i => i.IngredientId == request.IngredientId);
            if (ingredient == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Ingredient", request.IngredientId.ToString());
            }

            // Check duplicate
            var exists = await _unitOfWork.Repository<FestivalIngredient>().GetAll()
                .AnyAsync(fi => fi.FestivalId == request.FestivalId && fi.IngredientId == request.IngredientId);
            if (exists)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "FestivalIngredient đã tồn tại", null);
            }

            var newFestivalIngredient = new FestivalIngredient
            {
                FestivalId = request.FestivalId,
                IngredientId = request.IngredientId,
                QuantityAvailable = request.QuantityAvailable,
                SpecialPrice = request.SpecialPrice ?? ingredient.PricePerUnit,
                Status = StatusFestivalIngredient.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<FestivalIngredient>().InsertAsync(newFestivalIngredient);
            await _unitOfWork.CommitAsync();

            return newFestivalIngredient;
        }
        public async Task<FestivalIngredient> UpdateFestivalIngredientAsync(int id, int? festivalId, int? ingredientId, int? quantityAvailable, decimal? specialPrice, string? status)
        {
            var item = await _unitOfWork.Repository<FestivalIngredient>()
                .GetAll()
                .FirstOrDefaultAsync(fi => fi.FestivalIngredientId == id);

            if (item == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalIngredient", id.ToString());
            }

            if (festivalId.HasValue) item.FestivalId = festivalId.Value;
            if (ingredientId.HasValue) item.IngredientId = ingredientId.Value;
            if (quantityAvailable.HasValue) item.QuantityAvailable = quantityAvailable.Value;
            if (specialPrice.HasValue) item.SpecialPrice = specialPrice.Value;
            
            if (!string.IsNullOrWhiteSpace(status)) item.Status = status.Trim();

            item.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return item;
        }
        public async Task UpdateFestivaIngredientStatusToRejectAsync(int id, string? rejectReason)
        {
            var entity = await _unitOfWork.Repository<FestivalIngredient>()
                .GetAll()
                .FirstOrDefaultAsync(fs => fs.FestivalIngredientId == id);

            if (entity == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalSchool", id.ToString());
            }
            entity.RejectReason = rejectReason;
            entity.Status = StatusFestivalIngredient.Rejected;
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
        }
        public async Task<List<FestivalIngredient>> SearchFestivalIngredientsAsync(
        int? festivalIngredientId, int? festivalId, int? ingredientId,
        string? status, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<FestivalIngredient>().GetAll()
                .Where(fi => !festivalIngredientId.HasValue || fi.FestivalIngredientId == festivalIngredientId)
                .Where(fi => !festivalId.HasValue || fi.FestivalId == festivalId)
                .Where(fi => !ingredientId.HasValue || fi.IngredientId == ingredientId)
                .Where(fi => string.IsNullOrWhiteSpace(status) || fi.Status == status.Trim());

            int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);
            var results = await query.ToListAsync();

          

            return results;
        }

        public async Task DeleteFestivalIngredientAsync(int id)
        {
            var item = await _unitOfWork.Repository<FestivalIngredient>()
                .GetAll()
                .FirstOrDefaultAsync(fi => fi.FestivalIngredientId == id);

            if (item == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalIngredient", id.ToString());
            }

            _unitOfWork.Repository<FestivalIngredient>().Delete(item);
            await _unitOfWork.CommitAsync();
        }

    }
}
