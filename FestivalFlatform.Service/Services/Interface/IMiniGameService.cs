using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IMiniGameService
    {
        Task<Minigame> CreateMinigameAsync(MinigameCreateRequest request);
        Task<bool> DeleteMinigameAsync(int gameId);
        Task<List<Minigame>> SearchMinigamesAsync(
        int? gameId, int? boothId, string? gameName, string? gameType, string? status,
        int? pageNumber, int? pageSize);
        Task<Minigame> UpdateMinigameAsync(
        int gameId,
        int boothId,
        string gameName,
        string? gameType,
        string? rules,
        int? rewardPoints,
        string? status);
    }
}
