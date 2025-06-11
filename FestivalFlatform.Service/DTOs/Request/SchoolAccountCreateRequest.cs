using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class SchoolAccountCreateRequest
    {
        public int SchoolId { get; set; }
        public int AccountId { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
    }
}
