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

        public async Task<FestivalParticipant> CreateAsync(FestivalParticipantCreateRequest request)
        {
            var fest = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .FirstOrDefaultAsync(f => f.FestivalId == request.FestivalId);

            if (fest == null)
                throw new CrudException(HttpStatusCode.NotFound, "Festival không tồn tại", request.FestivalId.ToString());

          
            var accExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);
            if (!accExists)
                throw new CrudException(HttpStatusCode.NotFound, "Account không tồn tại", request.AccountId.ToString());

          
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

           
            fest.TotalRegisteredParticipants += 1;
            fest.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return participant;
        }




        public async Task<List<FestivalParticipant>> SearchFestivalParticipantsAsync(
     int? participantId,
     int? festivalId,
     int? accountId,
     int? pageNumber,
     int? pageSize)
        {
            var query = _unitOfWork.Repository<FestivalParticipant>()
                .GetAll()
                .Include(fp => fp.Account)
                .Include(fp => fp.Festival)
                .AsQueryable();

       
            query = query
                .Where(fp => !participantId.HasValue || participantId == 0 || fp.Id == participantId.Value)
                .Where(fp => !festivalId.HasValue || festivalId == 0 || fp.FestivalId == festivalId.Value)
                .Where(fp => !accountId.HasValue || accountId == 0 || fp.AccountId == accountId.Value);

      
            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize)
            //             .Take(currentSize);

            var participants = await query
                .OrderByDescending(fp => fp.RegisteredAt)
                .ToListAsync();

            return participants;
        }


        public async Task<bool> DeleteAsync(FestivalParticipantCreateRequest request)
        {
          
            var participant = await _unitOfWork.Repository<FestivalParticipant>()
                .GetAll()
                .FirstOrDefaultAsync(fp => fp.FestivalId == request.FestivalId && fp.AccountId == request.AccountId);

            if (participant == null)
                throw new CrudException(HttpStatusCode.NotFound,
                    "FestivalParticipant không tồn tại",
                    $"FestivalId={request.FestivalId}, AccountId={request.AccountId}");

     
            var fest = await _unitOfWork.Repository<Festival>()
                .GetAll()
                .FirstOrDefaultAsync(f => f.FestivalId == participant.FestivalId);

            if (fest != null && fest.TotalRegisteredParticipants > 0)
            {
                fest.TotalRegisteredParticipants -= 1;
                fest.UpdatedAt = DateTime.UtcNow;
            }

           
            _unitOfWork.Repository<FestivalParticipant>().Delete(participant);

            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
