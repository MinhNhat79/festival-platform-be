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
    public class FestivalParticipantService : IFestivalParticipantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FestivalParticipantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Create
        public async Task<FestivalParticipant> CreateAsync(FestivalParticipantCreateRequest request)
        {
            var fest = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .FirstOrDefaultAsync(f => f.FestivalId == request.FestivalId);

            if (fest == null)
                throw new CrudException(HttpStatusCode.NotFound, "Festival không tồn tại", request.FestivalId.ToString());

            // Check Account tồn tại
            var accExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);
            if (!accExists)
                throw new CrudException(HttpStatusCode.NotFound, "Account không tồn tại", request.AccountId.ToString());

            // Check trùng
            var existed = await _unitOfWork.Repository<FestivalParticipant>()
                .AnyAsync(fp => fp.FestivalId == request.FestivalId && fp.AccountId == request.AccountId);
            if (existed)
                throw new CrudException(HttpStatusCode.Conflict, "Người dùng đã tham gia festival này", request.AccountId.ToString());

            var participant = new FestivalParticipant
            {
                FestivalId = request.FestivalId,
                AccountId = request.AccountId,
                RegisteredAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<FestivalParticipant>().InsertAsync(participant);

            // ✅ Tăng totalRegisteredParticipants
            fest.TotalRegisteredParticipants += 1;
            fest.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return participant;
        }




        public async Task<List<FestivalParticipant>> SearchAsync(int? festivalId, int? accountId, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Repository<FestivalParticipant>()
                .GetAll()
                .Include(fp => fp.Account)
                .AsQueryable();

            if (festivalId.HasValue)
                query = query.Where(fp => fp.FestivalId == festivalId.Value);

            if (accountId.HasValue)
                query = query.Where(fp => fp.AccountId == accountId.Value);

            return await query
                .OrderByDescending(fp => fp.RegisteredAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<bool> DeleteAsync(FestivalParticipantCreateRequest request)
        {
            // Tìm participant theo FestivalId + AccountId
            var participant = await _unitOfWork.Repository<FestivalParticipant>()
                .GetAll()
                .FirstOrDefaultAsync(fp => fp.FestivalId == request.FestivalId && fp.AccountId == request.AccountId);

            if (participant == null)
                throw new CrudException(HttpStatusCode.NotFound,
                    "FestivalParticipant không tồn tại",
                    $"FestivalId={request.FestivalId}, AccountId={request.AccountId}");

            // Lấy festival liên quan
            var fest = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .FirstOrDefaultAsync(f => f.FestivalId == participant.FestivalId);

            if (fest != null && fest.TotalRegisteredParticipants > 0)
            {
                fest.TotalRegisteredParticipants -= 1; // ✅ Giảm số lượng
                fest.UpdatedAt = DateTime.UtcNow;
            }

            // Xóa participant
            _unitOfWork.Repository<FestivalParticipant>().Delete(participant);

            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
