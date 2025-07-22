using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface ISchoolAccountRelationService
    {
        Task<SchoolAccountRelation> CreateRelationAsync(CreateSchoolAccountRelationRequest request);
        Task<SchoolAccountRelation> UpdateRelationAsync(int id, string relationType);
        Task<List<SchoolAccountRelation>> SearchRelationsAsync(int? schoolId, int? accountId, string? relationType);
        Task<bool> DeleteRelationAsync(int id);
    }
}
