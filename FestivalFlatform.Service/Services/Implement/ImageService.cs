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
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public ImageService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Image> CreateImageAsync(CreateImageRequest request)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                throw new CrudException(HttpStatusCode.BadRequest, "ImageUrl không được để trống", nameof(request.ImageUrl));
            }

            if (request.MenuItemId.HasValue)
            {
                var menuItemExists = await _unitOfWork.Repository<MenuItem>()
                    .AnyAsync(x => x.ItemId == request.MenuItemId.Value);

                if (!menuItemExists)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy MenuItem", request.MenuItemId.Value.ToString());
                }
            }

            var image = new Image
            {
                MenuItemId = request.MenuItemId,
                ImageUrl = request.ImageUrl.Trim(),
                ImageName = request.ImageName?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Image>().InsertAsync(image);
            await _unitOfWork.CommitAsync();

            return image;
        }

        public async Task<Image> UpdateImageAsync(
    int imageId,
    string? imageUrl,
    string? imageName,
    int? festivalId,
    int? boothId,
    int? menuItemId)
        {
            var image = await _unitOfWork.Repository<Image>().GetAll()
                .FirstOrDefaultAsync(x => x.ImageId == imageId);

            if (image == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ảnh", imageId.ToString());

            // Gán imageUrl nếu có truyền
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                image.ImageUrl = imageUrl.Trim();
            }

            // Gán imageName nếu có truyền
            if (imageName != null)
            {
                image.ImageName = imageName.Trim();
            }

            // Chỉ kiểm tra và cập nhật nếu có truyền festivalId/boothId/menuItemId
            if (festivalId.HasValue)
            {
                var exists = await _unitOfWork.Repository<Festival>().AnyAsync(f => f.FestivalId == festivalId.Value);
                if (!exists)
                    throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy Festival", festivalId.Value.ToString());

                image.FestivalId = festivalId;
                image.BoothId = null;
                image.MenuItemId = null;
            }
            else if (boothId.HasValue)
            {
                var exists = await _unitOfWork.Repository<Booth>().AnyAsync(b => b.BoothId == boothId.Value);
                if (!exists)
                    throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy Booth", boothId.Value.ToString());

                image.BoothId = boothId;
                image.FestivalId = null;
                image.MenuItemId = null;
            }
            else if (menuItemId.HasValue)
            {
                var exists = await _unitOfWork.Repository<MenuItem>().AnyAsync(m => m.ItemId == menuItemId.Value);
                if (!exists)
                    throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy MenuItem", menuItemId.Value.ToString());

                image.MenuItemId = menuItemId;
                image.BoothId = null;
                image.FestivalId = null;
            }

            // Nếu không truyền gì thì giữ nguyên liên kết
            image.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return image;
        }


        public async Task<List<Image>> SearchImagesAsync(
        int? imageId,
        int? menuItemId,
        int? boothId,
        int? festivalId,
        string? imageUrl,
        string? imageName,
        int? pageNumber,
        int? pageSize)
        {
            var query = _unitOfWork.Repository<Image>().GetAll()
                .Where(x => !imageId.HasValue || x.ImageId == imageId.Value)
                .Where(x => !menuItemId.HasValue || x.MenuItemId == menuItemId.Value)
                .Where(x => !boothId.HasValue || x.BoothId == boothId.Value)
                .Where(x => !festivalId.HasValue || x.FestivalId == festivalId.Value)
                .Where(x => string.IsNullOrWhiteSpace(imageUrl) || x.ImageUrl.Contains(imageUrl.Trim()))
                .Where(x => string.IsNullOrWhiteSpace(imageName) || x.ImageName!.Contains(imageName.Trim()));

            //int currentPage = pageNumber.GetValueOrDefault(1);
            //int currentSize = pageSize.GetValueOrDefault(10);

            //query = query.Skip((currentPage - 1) * currentSize)
            //             .Take(currentSize);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await _unitOfWork.Repository<Image>().GetAll()
                .FirstOrDefaultAsync(x => x.ImageId == imageId);

            if (image == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ảnh", imageId.ToString());

            _unitOfWork.Repository<Image>().Delete(image);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<Image> AddImageToEntityAsync(AddImageToEntityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ImageUrl))
                throw new CrudException(HttpStatusCode.BadRequest, "ImageUrl không được để trống", nameof(request.ImageUrl));

            if (request.FestivalId is null && request.BoothId is null && request.MenuItemId is null)
                throw new CrudException(HttpStatusCode.BadRequest, "Cần cung cấp ít nhất một trong FestivalId, BoothId hoặc MenuItemId", "EntityId");

            // Validate đúng loại
            if (request.FestivalId.HasValue)
            {
                var exists = await _unitOfWork.Repository<Festival>().AnyAsync(x => x.FestivalId == request.FestivalId.Value);
                if (!exists)
                    throw new CrudException(HttpStatusCode.BadRequest, "Festival không tồn tại", request.FestivalId.Value.ToString());
            }

            if (request.BoothId.HasValue)
            {
                var exists = await _unitOfWork.Repository<Booth>().AnyAsync(x => x.BoothId == request.BoothId.Value);
                if (!exists)
                    throw new CrudException(HttpStatusCode.BadRequest, "Booth không tồn tại", request.BoothId.Value.ToString());
            }

            if (request.MenuItemId.HasValue)
            {
                var exists = await _unitOfWork.Repository<MenuItem>().AnyAsync(x => x.ItemId == request.MenuItemId.Value);
                if (!exists)
                    throw new CrudException(HttpStatusCode.BadRequest, "MenuItem không tồn tại", request.MenuItemId.Value.ToString());
            }

            var image = new Image
            {
                ImageUrl = request.ImageUrl.Trim(),
                ImageName = request.ImageName?.Trim(),
                FestivalId = request.FestivalId,
                BoothId = request.BoothId,
                MenuItemId = request.MenuItemId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Image>().InsertAsync(image);
            await _unitOfWork.CommitAsync();

            return image;
        }

    }
}
