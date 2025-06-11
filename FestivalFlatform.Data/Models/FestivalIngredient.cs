using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class FestivalIngredient
    {
        [Key]
        public int FestivalIngredientId { get; set; }

        [Required]
        public int FestivalId { get; set; }

        [Required]
        public int IngredientId { get; set; }

        public int QuantityAvailable { get; set; }
        public decimal? SpecialPrice { get; set; }
        public string Status { get; set; } = "available";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual Festival Festival { get; set; } = null!;
        public virtual Ingredient Ingredient { get; set; } = null!;
    }
}
