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
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Request> CreateRequestAsync(RequestCreateRequest request)
        {
            var accountExists = await _unitOfWork.Repository<Account>()
                .AnyAsync(a => a.AccountId == request.AccountId);
            if (!accountExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Account", request.AccountId.ToString());

            var newRequest = new Request
            {
                AccountId = request.AccountId,
                Type = request.Type.Trim(),
                Message = request.Message?.Trim(),
                Status = request.status,
                Data = request.Data,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Request>().InsertAsync(newRequest);
            await _unitOfWork.CommitAsync();
            return newRequest;
        }
        public async Task<List<Request>> SearchRequestsAsync(
            int? requestId = null,
            int? accountId = null,
            string? status = null,
            string? type = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            var query = _unitOfWork.Repository<Request>()
           .GetAll()
           .Include(r => r.Account) 
           .AsQueryable();

            if (requestId.HasValue)
                query = query.Where(r => r.Id == requestId.Value);

            if (accountId.HasValue)
                query = query.Where(r => r.AccountId == accountId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(r => r.Type.Contains(type.Trim()));

           

            return await query.ToListAsync();
        }


        public async Task<Request> UpdateRequestAsync(
          int requestId,
          string? status = null,
          string? message = null,
          string? type = null)
        {
            var request = await _unitOfWork.Repository<Request>()
                .GetAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Request", requestId.ToString());

            if (!string.IsNullOrWhiteSpace(status))
                request.Status = status.Trim();

            if (!string.IsNullOrWhiteSpace(message))
                request.Message = message.Trim();

            if (!string.IsNullOrWhiteSpace(type))
                request.Type = type.Trim();

            request.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return request;
        }

        public async Task<bool> DeleteRequestAsync(int requestId)
        {
            var request = await _unitOfWork.Repository<Request>()
                .GetAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Request", requestId.ToString());

            _unitOfWork.Repository<Request>().Delete(request);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
