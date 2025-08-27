using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class FestivalParticipant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FestivalId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(FestivalId))]
        public virtual Festival Festival { get; set; } = null!;

        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; } = null!;
    }
}
