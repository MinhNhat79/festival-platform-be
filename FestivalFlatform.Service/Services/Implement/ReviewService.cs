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
    public class ReviewService : IReviewService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public ReviewService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        private async Task UpdateFestivalAvgRatingAsync(int festivalId)
        {
            var reviews = await _unitOfWork.Repository<Review>().GetAll()
                .Where(r => r.FestivalId == festivalId)
                .ToListAsync();

            double avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            var festival = await _unitOfWork.Repository<Festival>()
                .FindAsync(f => f.FestivalId == festivalId);

            if (festival != null)
            {
                festival.Avr_Rating = Math.Round(avgRating, 2);
                festival.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.CommitAsync();
            }
        }
        public async Task<Review> CreateReviewAsync(ReviewCreateRequest request)
        {
            var festival = await _unitOfWork.Repository<Festival>()
                .FindAsync(f => f.FestivalId == request.FestivalId);

            if (festival == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy festival", request.FestivalId.ToString());

            if (festival.Status != "ongoing" && festival.Status != "completed")
                throw new CrudException(HttpStatusCode.BadRequest, "Chỉ có thể đánh giá khi lễ hội đang diễn ra hoặc đã kết thúc");

            var review = new Review
            {
                FestivalId = request.FestivalId,
                AccountId = request.AccountId,
                Rating = request.Rating,
                Comment = request.Comment,
                IsEdit = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Review>().InsertAsync(review);
            await _unitOfWork.CommitAsync();

            await UpdateFestivalAvgRatingAsync(request.FestivalId);

            return review;
        }
        public async Task<Review> UpdateReviewAsync(int reviewId, int? rating, string? comment)
        {
            var review = await _unitOfWork.Repository<Review>().FindAsync(r => r.ReviewId == reviewId);

            if (review == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy review", reviewId.ToString());

            if (rating.HasValue)
                review.Rating = rating.Value;

            if (!string.IsNullOrWhiteSpace(comment))
                review.Comment = comment;

            review.IsEdit = true;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            await UpdateFestivalAvgRatingAsync(review.FestivalId);

            return review;
        }
        public async Task<List<Review>> SearchReviewsAsync(int? reviewId, int? festivalId, int? accountId, int? rating, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Review>().GetAll()
                .Where(r => !reviewId.HasValue || r.ReviewId == reviewId.Value)
                .Where(r => !festivalId.HasValue || r.FestivalId == festivalId.Value)
                .Where(r => !accountId.HasValue || r.AccountId == accountId.Value)
                .Where(r => !rating.HasValue || r.Rating == rating.Value);

            
            // int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            // int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;
            // query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            return await query.ToListAsync();
        }
        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.Repository<Review>().FindAsync(r => r.ReviewId == reviewId);

            if (review == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy review", reviewId.ToString());

            int festivalId = review.FestivalId;

            _unitOfWork.Repository<Review>().Delete(review);
            await _unitOfWork.CommitAsync();

            await UpdateFestivalAvgRatingAsync(festivalId);
        }
    }
}
