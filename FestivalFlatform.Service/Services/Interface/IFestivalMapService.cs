using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IFestivalMapService
    {
        Task<FestivalMap> CreateFestivalMapAsync(FestivalMapCreateRequest request);
        Task<bool> DeleteFestivalMapAsync(int mapId);
        Task<List<FestivalMap>> SearchFestivalMapsAsync(int? mapId, int? festivalId, string? mapName, string? mapType, int? pageNumber, int? pageSize);
        Task<FestivalMap> UpdateFestivalMapAsync(int mapId, int? festivalId, string? mapName, string? mapType, string? mapUrl);
    }
}
