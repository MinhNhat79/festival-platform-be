using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class FestivalCommission
    {
        [Key]
        public int CommissionId { get; set; }

        [Required]
        public int FestivalId { get; set; }

        [ForeignKey(nameof(FestivalId))]
        public virtual Festival Festival { get; set; } = null!;

        public decimal Amount { get; set; }

        public double CommissionRate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
