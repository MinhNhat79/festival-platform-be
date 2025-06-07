using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public string IngredientName { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public string Unit { get; set; } = null!;

        [Required]
        public decimal PricePerUnit { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
