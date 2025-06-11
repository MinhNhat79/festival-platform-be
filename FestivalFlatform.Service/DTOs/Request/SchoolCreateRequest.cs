using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class SchoolCreateRequest
    {
        public string SchoolName { get; set; } = null!;
        public string? Address { get; set; }
        public string? ContactInfo { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
    }
}
