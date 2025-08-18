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
    public class FestivalMenuService : IFestivalMenuService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public FestivalMenuService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<FestivalMenu> CreateFestivalMenuAsync(FestivalMenuCreateRequest request)
        {
            var festivalExists = await _unitOfWork.Repository<Festival>()
                .GetAll().AnyAsync(f => f.FestivalId == request.FestivalId);

            if (!festivalExists)
                throw new Exception("FestivalId không tồn tại");

            var menu = new FestivalMenu
            {
                FestivalId = request.FestivalId,
                MenuName = request.MenuName,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _unitOfWork.Repository<FestivalMenu>().InsertAsync(menu);
            await _unitOfWork.CommitAsync();
            return menu;
        }
        public async Task<FestivalMenu> UpdateFestivalMenuAsync(int menuId, string? menuName, string? description)
        {
            var menu = await _unitOfWork.Repository<FestivalMenu>().FindAsync(fm => fm.MenuId == menuId);
            if (menu == null)
                throw new Exception("Không tìm thấy FestivalMenu");

            if (!string.IsNullOrEmpty(menuName)) menu.MenuName = menuName;
            if (description != null) menu.Description = description;

            menu.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return menu;
        }
        public async Task<List<FestivalMenu>> SearchFestivalMenusAsync(
        int? menuId, int? festivalId, string? menuName, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<FestivalMenu>().GetAll()
                .Where(m => !menuId.HasValue || m.MenuId == menuId.Value)
                .Where(m => !festivalId.HasValue || m.FestivalId == festivalId.Value)
                .Where(m => string.IsNullOrWhiteSpace(menuName) || m.MenuName.Contains(menuName.Trim()));

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

         

            return result;
        }

        public async Task<bool> DeleteFestivalMenuAsync(int menuId)
        {
            var menu = await _unitOfWork.Repository<FestivalMenu>().FindAsync(fm => fm.MenuId == menuId);
            if (menu == null)
                throw new Exception("FestivalMenu không tồn tại");

            _unitOfWork.Repository<FestivalMenu>().Delete(menu);
            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
