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
        public string Status { get; set; } = "pending";

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovalDate { get; set; }

        public int PointsBalance { get; set; } = 0;
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public virtual ICollection<Minigame> Minigames { get; set; } = new List<Minigame>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<PointsTransaction> PointsTransactions { get; set; } = new List<PointsTransaction>();
    }
}
