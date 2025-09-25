using System;
using System.Collections.Generic;
using FestivalFlatform.Data.Models;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class UpdateFestivalRequest
    {
        public int FestivalId { get; set; }
        public int? SchoolId { get; set; }
        public string? FestivalName { get; set; }
        public string? Theme { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public string? Location { get; set; }
        public int? MaxFoodBooths { get; set; }
        public int? MaxBeverageBooths { get; set; }

   
        public int? RegisteredFoodBooths { get; set; }
        public int? RegisteredBeverageBooths { get; set; }

        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? CancellationReason { get; set; }

        public List<Image>? Images { get; set; }
        public List<FestivalMap>? FestivalMaps { get; set; }
        public List<FestivalMenu>? FestivalMenus { get; set; }
    }
}
