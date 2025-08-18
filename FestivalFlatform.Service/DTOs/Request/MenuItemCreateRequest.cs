using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class MenuItemCreateRequest
    {
        [Required]
        public int MenuId { get; set; }

        [Required]
        public string ItemName { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public string ItemType { get; set; } = null!; 

      
        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }

    }

}
