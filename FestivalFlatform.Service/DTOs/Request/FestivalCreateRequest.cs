using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class FestivalCreateRequest
    {
        public int OrganizerSchoolId { get; set; }
        public string FestivalName { get; set; } = null!;
        public string? Theme { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public string? Location { get; set; }
        public int MaxFoodBooths { get; set; }
        public int MaxBeverageBooths { get; set; }
        public string? Description {  get; set; } 
    }
}
