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
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Comment> CreateCommentAsync(CommentCreateRequest request)
        {
            var comment = new Comment
            {
                ReviewId = request.ReviewId,
                AccountId = request.AccountId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                IsEdit = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Comment>().InsertAsync(comment);
            await _unitOfWork.CommitAsync();

            return comment;
        }

        public async Task<Comment> UpdateCommentAsync(int commentId, string content)
        {
            var comment = await _unitOfWork.Repository<Comment>().FindAsync(c => c.CommentId == commentId);
            if (comment == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy comment", commentId.ToString());
            }

            comment.Content = content;
            comment.IsEdit = true;
            comment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return comment;
        }

        public async Task<List<Comment>> SearchCommentsAsync(int? commentId, int? reviewId, int? accountId, int? parentCommentId, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Comment>().GetAll()
                .Include(c => c.Account)
                .Include(c => c.Review)
                .AsQueryable();

            query = query
                .Where(c => !commentId.HasValue || commentId == 0 || c.CommentId == commentId.Value)
                .Where(c => !reviewId.HasValue || reviewId == 0 || c.ReviewId == reviewId.Value)
                .Where(c => !accountId.HasValue || accountId == 0 || c.AccountId == accountId.Value)
                .Where(c => !parentCommentId.HasValue || parentCommentId == 0 || c.ParentCommentId == parentCommentId.Value);

            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return comments;
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _unitOfWork.Repository<Comment>().FindAsync(c => c.CommentId == commentId);
            if (comment == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy comment", commentId.ToString());
            }

            _unitOfWork.Repository<Comment>().Delete(comment);
            await _unitOfWork.CommitAsync();
        }
    }

}
