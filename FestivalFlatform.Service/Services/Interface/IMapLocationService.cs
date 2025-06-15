using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IMapLocationService
    {
        Task<MapLocation> CreateMapLocationAsync(MapLocationCreateRequest request);
        Task<bool> DeleteMapLocationAsync(int locationId);
        Task<List<MapLocation>> SearchMapLocationsAsync(
        int? locationId,
        int? mapId,
        string? locationName,
        string? locationType,
        int? pageNumber, int? pageSize);
        Task<MapLocation> UpdateMapLocationAsync(int locationId, string? locationName, string? locationType, string? coordinates, bool? isOccupied);
    }
}
