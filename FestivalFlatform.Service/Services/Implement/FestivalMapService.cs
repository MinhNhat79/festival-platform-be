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
    public class FestivalMapService : IFestivalMapService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public FestivalMapService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<FestivalMap> CreateFestivalMapAsync(FestivalMapCreateRequest request)
        {
            // Kiểm tra Festival có tồn tại không
            var exists = _unitOfWork.Repository<Festival>()
                .GetAll()
                .Any(f => f.FestivalId == request.FestivalId);

            if (!exists)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Festival không tồn tại", request.FestivalId.ToString());
            }

            var map = new FestivalMap
            {
                FestivalId = request.FestivalId,
                MapName = request.MapName,
                MapType = request.MapType,
                MapUrl = request.MapUrl,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = null
            };

            await _unitOfWork.Repository<FestivalMap>().InsertAsync(map);
            await _unitOfWork.CommitAsync();

            return map;
        }

        public async Task<FestivalMap> UpdateFestivalMapAsync(int mapId, int? festivalId, string? mapName, string? mapType, string? mapUrl)
        {
            var map = await _unitOfWork.Repository<FestivalMap>().FindAsync(fm => fm.MapId == mapId);
            if (map == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Festival", mapId.ToString());
            }

         

            if (!string.IsNullOrWhiteSpace(mapName)) map.MapName = mapName.Trim();
            if (!string.IsNullOrWhiteSpace(mapType)) map.MapType = mapType.Trim();
            if (!string.IsNullOrWhiteSpace(mapUrl)) map.MapUrl = mapUrl.Trim();

            map.LastUpdated = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            return map;
        }

        public async Task<List<FestivalMap>> SearchFestivalMapsAsync(int? mapId, int? festivalId, string? mapName, string? mapType, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<FestivalMap>().GetAll()
                .Where(x => !mapId.HasValue || x.MapId == mapId.Value)
                .Where(x => !festivalId.HasValue || x.FestivalId == festivalId.Value)
                .Where(x => string.IsNullOrWhiteSpace(mapName) || x.MapName.Contains(mapName))
                .Where(x => string.IsNullOrWhiteSpace(mapType) || x.MapType.Contains(mapType));

            //int currentPage = pageNumber.HasValue && pageNumber > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize).Take(currentSize);

            var result = await query.ToListAsync();

         
            return result;
        }
        public async Task<bool> DeleteFestivalMapAsync(int mapId)
        {
            var map = await _unitOfWork.Repository<FestivalMap>().FindAsync(fm => fm.MapId == mapId);
            if (map == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy FestivalMap", mapId.ToString());
            }

            _unitOfWork.Repository<FestivalMap>().Delete(map);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
