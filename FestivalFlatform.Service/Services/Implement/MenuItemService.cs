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
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public MenuItemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MenuItem> CreateMenuItemAsync(MenuItemCreateRequest request)
        {
       
            var menuExists = await _unitOfWork.Repository<FestivalMenu>()
                .GetAll()
                .AnyAsync(m => m.MenuId == request.MenuId);

            if (!menuExists)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy FestivalMenu", request.MenuId.ToString());
            }

          
            var normalizedType = request.ItemType.Trim().ToLower();
            if (normalizedType != StatusMenuItem.food && normalizedType != StatusMenuItem.beverage)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Loại món phải là 'food' hoặc 'beverage'", request.ItemType);
            }

            var item = new MenuItem
            {
                MenuId = request.MenuId,
                ItemName = request.ItemName.Trim(),
                Description = request.Description?.Trim(),
                ItemType = normalizedType,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<MenuItem>().InsertAsync(item);
            await _unitOfWork.CommitAsync();

            return item;
        }
        public async Task<MenuItem> UpdateMenuItemAsync(
        int itemId,
        int menuId,
        string itemName,
        string? description,
        string itemType,
        decimal minPrice,
        decimal maxPrice)
        {
            var item = await _unitOfWork.Repository<MenuItem>().GetAll()
                .FirstOrDefaultAsync(x => x.ItemId == itemId);

            if (item == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItem", itemId.ToString());
            }

       
            var menuExists = await _unitOfWork.Repository<FestivalMenu>()
                .GetAll()
                .AnyAsync(f => f.MenuId == menuId);

            if (!menuExists)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy FestivalMenu", menuId.ToString());
            }

           
            var type = itemType.Trim().ToLower();
            if (type != StatusMenuItem.food && type != StatusMenuItem.beverage)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Loại món phải là 'food' hoặc 'beverage'", itemType);
            }

            item.MenuId = menuId;
            item.ItemName = itemName.Trim();
            item.Description = description?.Trim();
            item.ItemType = type;
            item.MinPrice = minPrice;
            item.MaxPrice = maxPrice;
            item.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return item;
        }
        public async Task<List<MenuItem>> SearchMenuItemsAsync(
        int? itemId, int? menuId, string? itemName, string? itemType,
        int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<MenuItem>().GetAll()
                .Where(i => !itemId.HasValue || i.ItemId == itemId.Value)
                .Where(i => !menuId.HasValue || i.MenuId == menuId.Value)
                .Where(i => string.IsNullOrWhiteSpace(itemName) || i.ItemName.Contains(itemName.Trim()))
                .Where(i => string.IsNullOrWhiteSpace(itemType) || i.ItemType == itemType.Trim().ToLower());

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

         

            return result;
        }
        public async Task DeleteMenuItemAsync(int itemId)
        {
            var item = await _unitOfWork.Repository<MenuItem>().GetAll()
                .FirstOrDefaultAsync(i => i.ItemId == itemId);

            if (item == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItem", itemId.ToString());
            }

            _unitOfWork.Repository<MenuItem>().Delete(item);
            await _unitOfWork.CommitAsync();
        }



    }
}
