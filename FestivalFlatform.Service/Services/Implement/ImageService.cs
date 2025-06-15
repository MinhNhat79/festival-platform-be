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
        int? menuItemId,
        string imageUrl,
        string? imageName)
        {
            var image = await _unitOfWork.Repository<Image>().GetAll()
                .FirstOrDefaultAsync(x => x.ImageId == imageId);

            if (image == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ảnh", imageId.ToString());

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new CrudException(HttpStatusCode.BadRequest, "ImageUrl không được để trống", nameof(imageUrl));

            if (menuItemId.HasValue)
            {
                var menuItemExists = await _unitOfWork.Repository<MenuItem>()
                    .AnyAsync(x => x.ItemId == menuItemId.Value);

                if (!menuItemExists)
                    throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy MenuItem", menuItemId.Value.ToString());
            }

            image.MenuItemId = menuItemId;
            image.ImageUrl = imageUrl.Trim();
            image.ImageName = imageName?.Trim();
            image.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return image;
        }
        public async Task<List<Image>> SearchImagesAsync(
        int? imageId,
        int? menuItemId,
        string? imageUrl,
        string? imageName,
        int? pageNumber,
        int? pageSize)
        {
            var query = _unitOfWork.Repository<Image>().GetAll()
                .Where(x => !imageId.HasValue || x.ImageId == imageId.Value)
                .Where(x => !menuItemId.HasValue || x.MenuItemId == menuItemId.Value)
                .Where(x => string.IsNullOrWhiteSpace(imageUrl) || x.ImageUrl.Contains(imageUrl.Trim()))
                .Where(x => string.IsNullOrWhiteSpace(imageName) || x.ImageName!.Contains(imageName.Trim()));

            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);

            query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

            if (result.Count == 0)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ảnh phù hợp", imageId?.ToString() ?? "No filter");

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

    }
}
