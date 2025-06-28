using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Booth
    {
        [Key]
        public int BoothId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int FestivalId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public string BoothName { get; set; } = null!;

        public string? BoothType { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Status { get; set; } // pending, approved, rejected, active

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovalDate { get; set; }

        public int PointsBalance { get; set; } = 0;

        public DateTime? UpdatedAt { get; set; }

        public string? Note { get; set; }

        // ✅ Navigation Properties
        public virtual StudentGroup StudentGroup { get; set; } = null!;
        public virtual Festival Festival { get; set; } = null!;
        public virtual MapLocation Location { get; set; } = null!;

        public virtual ICollection<Minigame> Minigames { get; set; } = new List<Minigame>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<PointsTransaction> PointsTransactions { get; set; } = new List<PointsTransaction>();
        public virtual ICollection<BoothMenuItem> BoothMenuItems { get; set; } = new List<BoothMenuItem>();

    }
}
