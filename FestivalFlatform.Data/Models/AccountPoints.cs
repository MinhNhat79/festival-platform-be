using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class AccountPoints
    {
        [Key]
        public int AccountPointsId { get; set; }

        [Required]
        public int AccountId { get; set; }

        public int PointsBalance { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public virtual Account Account { get; set; } = null!;
    }
}
