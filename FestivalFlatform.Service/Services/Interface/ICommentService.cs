using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(CommentCreateRequest request);
        Task<Comment> UpdateCommentAsync(int commentId, string content);
        Task<List<Comment>> SearchCommentsAsync(int? commentId, int? reviewId, int? accountId, int? parentCommentId, int? pageNumber, int? pageSize);
        Task DeleteCommentAsync(int commentId);
    }
}
