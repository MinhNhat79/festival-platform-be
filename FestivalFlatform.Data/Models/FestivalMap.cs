using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class FestivalMap
    {
        [Key]
        public int MapId { get; set; }

        [Required]
        public int FestivalId { get; set; }

        public string? MapName { get; set; }
        public string? MapType { get; set; }
        public string? MapUrl { get; set; }

        public DateTime? LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<MapLocation> Locations { get; set; } = new List<MapLocation>();

    }
}
