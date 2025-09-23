using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class AccountWalletHistory
    {
        [Key]
        public int HistoryId { get; set; }

        public int AccountId { get; set; }

        [Required]
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Type { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Account Account { get; set; } = null!;
    }
}
