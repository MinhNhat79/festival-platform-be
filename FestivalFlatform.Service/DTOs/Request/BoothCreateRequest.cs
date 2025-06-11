using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{

    public class BoothCreateRequest
    {
        public int GroupId { get; set; }
        public int FestivalId { get; set; }
        public int LocationId { get; set; }
        public string BoothName { get; set; } = null!;
        public string? BoothType { get; set; }
        public string? Description { get; set; }
    }
}
