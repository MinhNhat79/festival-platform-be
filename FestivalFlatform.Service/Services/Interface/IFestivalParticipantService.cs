using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IFestivalParticipantService
    {
        Task<FestivalParticipant> CreateAsync(FestivalParticipantCreateRequest request);

        Task<List<FestivalParticipant>> SearchAsync(int? festivalId, int? accountId, int pageNumber, int pageSize);
        Task<bool> DeleteAsync(FestivalParticipantCreateRequest request);
    }
}
