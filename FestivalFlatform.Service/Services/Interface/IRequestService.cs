using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IRequestService
    {
        Task<Request> CreateRequestAsync(RequestCreateRequest request);
        Task<List<Request>> SearchRequestsAsync(
           int? requestId = null,
           int? accountId = null,
           string? status = null,
           string? type = null,
           int? pageNumber = null,
           int? pageSize = null);
        Task<Request> UpdateRequestAsync(
          int requestId,
          string? status = null,
          string? message = null,
          string? type = null);
        Task<bool> DeleteRequestAsync(int requestId);
    }
}
