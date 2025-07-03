using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface ISchoolService
    {
        Task<School> CreateSchoolAsync(SchoolCreateRequest request);
        Task<School> UpdateSchoolAsync(int schoolId, string? contactInfo, string? logoUrl, string? description);
        Task<List<School>> SearchSchoolsAsync(int? schoolId, int? accountId, string? schoolName, int? pageNumber, int? pageSize);
        Task DeleteSchoolAsync(int schoolId);
    }
}
