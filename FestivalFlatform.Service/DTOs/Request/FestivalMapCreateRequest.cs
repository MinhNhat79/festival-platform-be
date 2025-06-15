using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class FestivalMapCreateRequest
    {
        public int FestivalId { get; set; }
        public string? MapName { get; set; }
        public string? MapType { get; set; }
        public string? MapUrl { get; set; }
    }
}
