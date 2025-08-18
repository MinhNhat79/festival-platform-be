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
    public class BoothMenuItemService : IBoothMenuItemService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public BoothMenuItemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BoothMenuItem> CreateBoothMenuItemAsync(BoothMenuItemCreateRequest request)
        {

            var existed = await _unitOfWork.Repository<BoothMenuItem>()
          .GetAll()
          .FirstOrDefaultAsync(x => x.BoothId == request.BoothId && x.MenuItemId == request.MenuItemId);

            if (existed != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "BoothMenuItem đã tồn tại", $"BoothId: {request.BoothId}, MenuItemId: {request.MenuItemId}");
            }

            var booth = await _unitOfWork.Repository<Booth>().FindAsync(b => b.BoothId == request.BoothId);
            if (booth == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Booth", request.BoothId.ToString());
            }

            var menuItem = await _unitOfWork.Repository<MenuItem>().FindAsync(mi => mi.ItemId == request.MenuItemId);
            if (menuItem == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItem", request.MenuItemId.ToString());
            }

            // Kiểm tra custom price nếu được truyền vào
            decimal customPrice = request.CustomPrice ?? menuItem.MinPrice;

            if (customPrice < menuItem.MinPrice || customPrice > menuItem.MaxPrice)
            {
                throw new CrudException(HttpStatusCode.BadRequest,
                    "CustomPrice phải nằm trong khoảng min và max price",
                    $"MinPrice: {menuItem.MinPrice}, MaxPrice: {menuItem.MaxPrice}, CustomPrice: {customPrice}");
            }

            var boothMenuItem = new BoothMenuItem
            {
                BoothId = request.BoothId,
                MenuItemId = request.MenuItemId,
                CustomPrice = customPrice,
                QuantityLimit = request.QuantityLimit,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<BoothMenuItem>().InsertAsync(boothMenuItem);
            await _unitOfWork.CommitAsync();

            return boothMenuItem;
        }

        public async Task<BoothMenuItem> UpdateBoothMenuItemAsync(int id, decimal? customPrice, int? quantityLimit,int? remainingQuantity,  string? status)
        {
            var item = await _unitOfWork.Repository<BoothMenuItem>().FindAsync(bmi => bmi.BoothMenuItemId == id);
            if (item == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy BoothMenuItem", id.ToString());
            }

            if (customPrice.HasValue)
                item.CustomPrice = customPrice;
            if (quantityLimit.HasValue)
                item.QuantityLimit = quantityLimit;
            if (remainingQuantity.HasValue)
                item.RemainingQuantity = remainingQuantity.Value;
            if (!string.IsNullOrWhiteSpace(status))
                item.Status = status;

            await _unitOfWork.CommitAsync();

            return item;
        }
        public async Task<List<BoothMenuItem>> SearchBoothMenuItemsAsync(
        int? boothMenuItemId, int? boothId, int? menuItemId, string? status, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<BoothMenuItem>().GetAll()
                .Include(x => x.MenuItem)
                .Where(x => !boothMenuItemId.HasValue || x.BoothMenuItemId == boothMenuItemId.Value)
                .Where(x => !boothId.HasValue || x.BoothId == boothId.Value)
                .Where(x => !menuItemId.HasValue || x.MenuItemId == menuItemId.Value)
                .Where(x => string.IsNullOrWhiteSpace(status) || x.Status == status.Trim());

            //int currentPage = pageNumber.GetValueOrDefault(1);
            //int currentSize = pageSize.GetValueOrDefault(10);

            //var result = await query
            //    .Skip((currentPage - 1) * currentSize)
            //    .Take(currentSize)
            //    .ToListAsync();


            return await query.ToListAsync();
        }
        public async Task DeleteBoothMenuItemAsync(int id)
        {
            var item = await _unitOfWork.Repository<BoothMenuItem>().GetAll().FirstOrDefaultAsync(bmi => bmi.BoothMenuItemId == id);
            if (item == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy BoothMenuItem", id.ToString());
            }

            _unitOfWork.Repository<BoothMenuItem>().Delete(item);
            await _unitOfWork.CommitAsync();
        }


    }
}
