using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(ReviewCreateRequest request);
        Task<Review> UpdateReviewAsync(int reviewId, int? rating, string? comment);
        Task<List<Review>> SearchReviewsAsync(int? reviewId, int? festivalId, int? accountId, int? rating, int? pageNumber, int? pageSize);
        Task DeleteReviewAsync(int reviewId);
    }
}
