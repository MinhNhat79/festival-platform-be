using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class PointsTransaction
    {
        [Key]
        public int PointsTxId { get; set; }

        [Required]
        public int AccountId { get; set; }

        public int? BoothId { get; set; }
        public int? GameId { get; set; }


        [Required]
        public int PointsAmount { get; set; }

        [Required]
        public string TransactionType { get; set; } = null!; // earned, spent

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public virtual Account Account { get; set; } = null!;

        public virtual Booth Booth { get; set; } = null!;

        public virtual Minigame Minigame { get; set; } = null!;
    }
}
