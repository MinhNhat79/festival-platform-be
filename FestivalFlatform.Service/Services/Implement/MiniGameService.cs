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
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class MiniGameService : IMiniGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public MiniGameService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<Minigame> CreateMinigameAsync(MinigameCreateRequest request)
        {
           
            var boothExists = await _unitOfWork.Repository<Booth>()
                .AnyAsync(b => b.BoothId == request.BoothId);

            if (!boothExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Booth", request.BoothId.ToString());

            var minigame = new Minigame
            {
                BoothId = request.BoothId,
                GameName = request.GameName.Trim(),
                GameType = "quiz",
                Rules = request.Rules?.Trim(),
                RewardPoints = request.RewardPoints ?? 0,
                Status = StatusGame.active,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Minigame>().InsertAsync(minigame);
            await _unitOfWork.CommitAsync();

            return minigame;
        }

        public async Task<Minigame> UpdateMinigameAsync(
        int gameId,
        int boothId,
        string gameName,
        string? gameType,
        string? rules,
        int? rewardPoints,
        string? status)
        {
            var minigame = await _unitOfWork.Repository<Minigame>()
                .GetAll()
                .FirstOrDefaultAsync(g => g.GameId == gameId);

            if (minigame == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Minigame", gameId.ToString());

           
            var boothExists = await _unitOfWork.Repository<Booth>()
                .AnyAsync(b => b.BoothId == boothId);
            if (!boothExists)
                throw new CrudException(HttpStatusCode.BadRequest, "Booth không tồn tại", boothId.ToString());

            minigame.BoothId = boothId;
            minigame.GameName = gameName.Trim();
            minigame.GameType = string.IsNullOrWhiteSpace(gameType) ? "quiz" : gameType.Trim().ToLower();
            minigame.Rules = rules?.Trim();
            minigame.RewardPoints = rewardPoints ?? 0;
            minigame.Status = string.IsNullOrWhiteSpace(status) ? "active" : status.Trim().ToLower();
            minigame.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return minigame;
        }
        public async Task<List<Minigame>> SearchMinigamesAsync(
        int? gameId, int? boothId, string? gameName, string? gameType, string? status,
        int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Minigame>().GetAll()
                .Where(m => !gameId.HasValue || m.GameId == gameId)
                .Where(m => !boothId.HasValue || m.BoothId == boothId)
                .Where(m => string.IsNullOrWhiteSpace(gameName) || m.GameName.Contains(gameName.Trim()))
                .Where(m => string.IsNullOrWhiteSpace(gameType) || m.GameType == gameType.Trim().ToLower())
                .Where(m => string.IsNullOrWhiteSpace(status) || m.Status == status.Trim().ToLower());

            //int currentPage = pageNumber.GetValueOrDefault(1);
            //int currentSize = pageSize.GetValueOrDefault(10);

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

        

            return result;
        }

        public async Task<bool> DeleteMinigameAsync(int gameId)
        {
            var minigame = await _unitOfWork.Repository<Minigame>()
                .FindAsync(m => m.GameId == gameId);

            if (minigame == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Minigame", gameId.ToString());

            _unitOfWork.Repository<Minigame>().Delete(minigame);
            await _unitOfWork.CommitAsync();

            return true;
        }

    }
}
