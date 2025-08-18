using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class BoothMenuItem
    {
        [Key]
        public int BoothMenuItemId { get; set; }

        [Required]
        public int BoothId { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        public decimal? CustomPrice { get; set; }

        public int? QuantityLimit { get; set; }

        public int? RemainingQuantity { get; set; }

        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Booth Booth { get; set; } = null!;
        public virtual MenuItem MenuItem { get; set; } = null!;
    }
}
