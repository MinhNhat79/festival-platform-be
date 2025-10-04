using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class BoothUpdateRequest
    {
        public string? BoothName { get; set; }
        public string? BoothType { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? IsWithdraw { get; set; }

        public MapLocationUpdateRequest? Location { get; set; }
        public List<BoothMenuItemUpdateRequest>? BoothMenuItems { get; set; }
    }

    public class MapLocationUpdateRequest
    {
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? LocationType { get; set; }
        public bool? IsOccupied { get; set; }
        public string? Coordinates { get; set; }
    }

    public class BoothMenuItemUpdateRequest
    {
        public int BoothMenuItemId { get; set; }
        public decimal? CustomPrice { get; set; }
        public int? QuantityLimit { get; set; }
        public string? Status { get; set; }
        public string? ImageUrl { get; set; }
    }


}
