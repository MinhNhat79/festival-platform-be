using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IFestivalSchoolService
    {
        Task<FestivalSchool> CreateFestivalSchoolAsync(FestivalSchoolCreateRequest request);
        Task<FestivalSchool> UpdateFestivalSchoolAsync(
        int id, int? festivalId, int? schoolId);
        Task<List<FestivalSchool>> SearchFestivalSchoolsAsync(
        int? festivalSchoolId, int? festivalId, int? schoolId, string? status, int? pageNumber, int? pageSize);
        Task DeleteFestivalSchoolAsync(int id);
        Task UpdateFestivalSchoolStatusToRejectAsync(int festivalSchoolId);

        Task UpdateFestivalSchoolStatusToApproveAsync(int festivalSchoolId);
    }
}
