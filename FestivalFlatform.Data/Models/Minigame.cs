using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Minigame
    {
        [Key]
        public int GameId { get; set; }

        [Required]
        public int BoothId { get; set; }

        [Required]
        public string GameName { get; set; } = null!;

        public string GameType { get; set; } = "quiz";
        public string? Rules { get; set; }

        public int RewardPoints { get; set; } = 0;
        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<Question> Questions { get; set; } = new HashSet<Question>();

        public virtual Booth Booth { get; set; } = null!;

    }
}
