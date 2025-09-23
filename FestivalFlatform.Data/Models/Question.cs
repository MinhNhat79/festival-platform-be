using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public string QuestionText { get; set; } = null!;

        [Required]
        public string Options { get; set; } = null!;

        [Required]
        public string CorrectAnswer { get; set; } = null!;

        public int Points { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Minigame Game { get; set; } = null!;
    }
}
