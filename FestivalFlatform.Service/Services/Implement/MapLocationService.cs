using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class MapLocationService : IMapLocationService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public MapLocationService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MapLocation> CreateMapLocationAsync(MapLocationCreateRequest request)
        {
            // Kiểm tra MapId có tồn tại không
            var mapExists =  _unitOfWork.Repository<FestivalMap>()
                .GetAll()
                .Any(fm => fm.MapId == request.MapId);

            if (!mapExists)
            {
                throw new CrudException(HttpStatusCode.NotFound, "MapId không tồn tại", request.MapId.ToString());
            }

            var location = new MapLocation
            {
                MapId = request.MapId,
                LocationName = request.LocationName,
                LocationType = request.LocationType,
                Coordinates = request.Coordinates,
                IsOccupied = request.IsOccupied,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _unitOfWork.Repository<MapLocation>().InsertAsync(location);
            await _unitOfWork.CommitAsync();

            return location;
        }

        public async Task<MapLocation> UpdateMapLocationAsync(int locationId, string? locationName, string? locationType, string? coordinates, bool? isOccupied)
        {
            var location = await _unitOfWork.Repository<MapLocation>().GetAll()
                .FirstOrDefaultAsync(x => x.LocationId == locationId);

            if (location == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "LocationId không tồn tại", locationId.ToString());
            }

            if (!string.IsNullOrWhiteSpace(locationName))
                location.LocationName = locationName;

            if (!string.IsNullOrWhiteSpace(locationType))
                location.LocationType = locationType;

            if (!string.IsNullOrWhiteSpace(coordinates))
                location.Coordinates = coordinates;

            if (isOccupied.HasValue)
                location.IsOccupied = isOccupied.Value;

            location.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return location;
        }
        public async Task<List<MapLocation>> SearchMapLocationsAsync(
        int? locationId,
        int? mapId,
        string? locationName,
        string? locationType,
         int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<MapLocation>().GetAll().AsQueryable();

            if (locationId.HasValue)
                query = query.Where(x => x.LocationId == locationId.Value);

            if (mapId.HasValue)
                query = query.Where(x => x.MapId == mapId.Value);

            if (!string.IsNullOrWhiteSpace(locationName))
                query = query.Where(x => x.LocationName.Contains(locationName));

            if (!string.IsNullOrWhiteSpace(locationType))
                query = query.Where(x => x.LocationType != null && x.LocationType.Contains(locationType));

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteMapLocationAsync(int locationId)
        {
            var location = await _unitOfWork.Repository<MapLocation>().GetAll()
                .FirstOrDefaultAsync(x => x.LocationId == locationId);

            if (location == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "LocationId không tồn tại", locationId.ToString());
            }

            _unitOfWork.Repository<MapLocation>().Delete(location);
            await _unitOfWork.CommitAsync();

            return true;
        }

    }
}
