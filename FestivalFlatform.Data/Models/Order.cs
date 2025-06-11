using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public int BoothId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalAmount { get; set; }

        public int PointsUsed { get; set; } = 0;
        public decimal CashAmount { get; set; } = 0;

        public string Status { get; set; } = "pending";
        public string? Notes { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual Account Account { get; set; } = null!;
        public virtual Booth Booth { get; set; } = null!;



    }
}
