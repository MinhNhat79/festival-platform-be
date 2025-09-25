using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class AddImageToEntityRequest
    {
        public string ImageUrl { get; set; } = null!;
        public string? ImageName { get; set; }

        public int? FestivalId { get; set; }
        public int? BoothId { get; set; }
        public int? MenuItemId { get; set; }
        public int? BoothMenuItemId { get; set; }

    }
}
