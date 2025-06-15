using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreateImageRequest
    {
        public int? MenuItemId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        public string? ImageName { get; set; }
    }
}
