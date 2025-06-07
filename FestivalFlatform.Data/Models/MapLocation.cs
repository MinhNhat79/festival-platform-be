using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class MapLocation
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        public int MapId { get; set; }

        [Required]
        public string LocationName { get; set; } = null!;

        public string? LocationType { get; set; }
        public bool IsOccupied { get; set; } = false;
        public string? Coordinates { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
