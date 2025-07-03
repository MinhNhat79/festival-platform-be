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
    public class MenuItemIngredientService : IMenuItemIngredientService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public MenuItemIngredientService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MenuItemIngredient> CreateMenuItemIngredientAsync(CreateMenuItemIngredientRequest request)
        {
           

            // Kiểm tra tồn tại MenuItem
            var menuItemExists = await _unitOfWork.Repository<MenuItem>()
                .AnyAsync(m => m.ItemId == request.ItemId);
            if (!menuItemExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItem", request.ItemId.ToString());

            // Kiểm tra tồn tại Ingredient
            var ingredientExists = await _unitOfWork.Repository<Ingredient>()
                .AnyAsync(i => i.IngredientId == request.IngredientId);
            if (!ingredientExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Ingredient", request.IngredientId.ToString());

            var entity = new MenuItemIngredient
            {
                ItemId = request.ItemId,
                IngredientId = request.IngredientId,
                Quantity = request.Quantity,
                Unit = request.Unit.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<MenuItemIngredient>().InsertAsync(entity);
            await _unitOfWork.CommitAsync();

            return entity;
        }
        public async Task<MenuItemIngredient> UpdateMenuItemIngredientAsync(
        int itemIngredientId,
        int itemId,
        int ingredientId,
        decimal quantity,
        string unit)
        {
            var entity = await _unitOfWork.Repository<MenuItemIngredient>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.ItemIngredientId == itemIngredientId);

            if (entity == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItemIngredient", itemIngredientId.ToString());

            // Kiểm tra MenuItem
            var itemExists = await _unitOfWork.Repository<MenuItem>()
                .AnyAsync(x => x.ItemId == itemId);
            if (!itemExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItem", itemId.ToString());

            // Kiểm tra Ingredient
            var ingExists = await _unitOfWork.Repository<Ingredient>()
                .AnyAsync(x => x.IngredientId == ingredientId);
            if (!ingExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Ingredient", ingredientId.ToString());

            // Gán giá trị mới
            entity.ItemId = itemId;
            entity.IngredientId = ingredientId;
            entity.Quantity = quantity;
            entity.Unit = unit.Trim();
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return entity;
        }

        public async Task<List<MenuItemIngredient>> SearchMenuItemIngredientsAsync(
        int? itemIngredientId,
        int? itemId,
        int? ingredientId,
        string? unit,
        int? pageNumber,
        int? pageSize)
        {
            var query = _unitOfWork.Repository<MenuItemIngredient>().GetAll()
                .Where(x => !itemIngredientId.HasValue || x.ItemIngredientId == itemIngredientId.Value)
                .Where(x => !itemId.HasValue || x.ItemId == itemId.Value)
                .Where(x => !ingredientId.HasValue || x.IngredientId == ingredientId.Value)
                .Where(x => string.IsNullOrWhiteSpace(unit) || x.Unit.Contains(unit.Trim()));

            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);

            var result = await query
                .Skip((currentPage - 1) * currentSize)
                .Take(currentSize)
                .ToListAsync();


            return result;


        }

        public async Task<bool> DeleteMenuItemIngredientAsync(int itemIngredientId)
        {
            var entity = await _unitOfWork.Repository<MenuItemIngredient>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.ItemIngredientId == itemIngredientId);

            if (entity == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItemIngredient", itemIngredientId.ToString());

            _unitOfWork.Repository<MenuItemIngredient>().Delete(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }


    }
}
