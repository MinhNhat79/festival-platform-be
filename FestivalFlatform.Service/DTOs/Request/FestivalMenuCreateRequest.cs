using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class FestivalMenuCreateRequest
    {
        public int FestivalId { get; set; }
        public string MenuName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
