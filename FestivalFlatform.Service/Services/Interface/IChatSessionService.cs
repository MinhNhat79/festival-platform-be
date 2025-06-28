using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IChatSessionService
    {
        Task<bool> DeleteChatSessionAsync(int sessionId);
        Task<List<ChatSession>> SearchChatSessionsAsync(
        int? sessionId, int? accountId,
        int? pageNumber, int? pageSize);
        Task<ChatSession> UpdateChatSessionAsync(int sessionId, DateTime? lastMessageAt);
        Task<ChatSession> CreateChatSessionAsync(ChatSessionCreateRequest request);
    }
}
