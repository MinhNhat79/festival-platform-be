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
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public OrderItemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderItem> CreateOrderItemAsync(OrderItemCreateRequest request)
        {
            var orderExists = await _unitOfWork.Repository<Order>()
                .AnyAsync(o => o.OrderId == request.OrderId);
            if (!orderExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Order", request.OrderId.ToString());

            var menuItemExists = await _unitOfWork.Repository<MenuItem>()
                .AnyAsync(m => m.ItemId == request.MenuItemId);
            if (!menuItemExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy MenuItem", request.MenuItemId.ToString());

            var newItem = new OrderItem
            {
                OrderId = request.OrderId,
                MenuItemId = request.MenuItemId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                Subtotal = request.UnitPrice * request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<OrderItem>().InsertAsync(newItem);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<OrderItem> UpdateOrderItemAsync(int id, int quantity, decimal unitPrice)
        {
            var item = await _unitOfWork.Repository<OrderItem>().GetAll()
                .FirstOrDefaultAsync(i => i.OrderItemId == id);
            if (item == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy OrderItem", id.ToString());

            if (quantity <= 0 || unitPrice < 0)
                throw new CrudException(HttpStatusCode.BadRequest, "Giá hoặc số lượng không hợp lệ");

            item.Quantity = quantity;
            item.UnitPrice = unitPrice;
            item.Subtotal = quantity * unitPrice;
            item.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return item;
        }

        public async Task<List<OrderItem>> SearchOrderItemsAsync(int? orderItemId, int? orderId, int? menuItemId, int? page, int? size)
        {
            var query = _unitOfWork.Repository<OrderItem>().GetAll()
                .Where(i => !orderItemId.HasValue || i.OrderItemId == orderItemId)
                .Where(i => !orderId.HasValue || i.OrderId == orderId)
                .Where(i => !menuItemId.HasValue || i.MenuItemId == menuItemId);

            //int currentPage = page.GetValueOrDefault(1);
            //int pageSize = size.GetValueOrDefault(10);

            return await query.ToListAsync();
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            var item = await _unitOfWork.Repository<OrderItem>().GetAll()
                .FirstOrDefaultAsync(i => i.OrderItemId == id);

            if (item == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy OrderItem", id.ToString());

            _unitOfWork.Repository<OrderItem>().Delete(item);
            await _unitOfWork.CommitAsync();
        }

    }
}
