using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatSessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ChatSession> CreateChatSessionAsync(ChatSessionCreateRequest request)
        {
            var accountExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);

            if (!accountExists)
                throw new CrudException(HttpStatusCode.NotFound, "Account không tồn tại", request.AccountId.ToString());

            var chatSession = new ChatSession
            {
                AccountId = request.AccountId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<ChatSession>().InsertAsync(chatSession);
            await _unitOfWork.CommitAsync();
            return chatSession;
        }

        public async Task<ChatSession> UpdateChatSessionAsync(int sessionId, DateTime? lastMessageAt)
        {
            var session = await _unitOfWork.Repository<ChatSession>()
                .GetAll()
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ChatSession", sessionId.ToString());

            session.LastMessageAt = lastMessageAt;
            session.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return session;
        }

        public async Task<List<ChatSession>> SearchChatSessionsAsync(
            int? sessionId, int? accountId,
            int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<ChatSession>().GetAll()
                .Where(s => !sessionId.HasValue || s.SessionId == sessionId.Value)
                .Where(s => !accountId.HasValue || s.AccountId == accountId.Value);

            //int currentPage = pageNumber.GetValueOrDefault(1);
            //int currentSize = pageSize.GetValueOrDefault(10);

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteChatSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.Repository<ChatSession>()
                .FindAsync(s => s.SessionId == sessionId);

            if (session == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ChatSession", sessionId.ToString());

            _unitOfWork.Repository<ChatSession>().Delete(session);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}

