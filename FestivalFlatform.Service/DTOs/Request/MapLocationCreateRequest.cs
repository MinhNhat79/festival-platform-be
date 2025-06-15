using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class MapLocationCreateRequest
    {
        public int MapId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? LocationType { get; set; }
        public bool IsOccupied { get; set; } = false;
        public string? Coordinates { get; set; }
    }
}
