using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IImageService
    {

        Task<Image> CreateImageAsync(CreateImageRequest request);
        Task<Image> UpdateImageAsync(
        int imageId,
        int? menuItemId,
        string imageUrl,
        string? imageName);
        Task<List<Image>> SearchImagesAsync(
       int? imageId,
       int? menuItemId,
       int? boothId,
       int? festivalId,
       string? imageUrl,
       string? imageName,
       int? pageNumber,
       int? pageSize);

        Task<bool> DeleteImageAsync(int imageId);
        Task<Image> AddImageToEntityAsync(AddImageToEntityRequest request);
    }
}
