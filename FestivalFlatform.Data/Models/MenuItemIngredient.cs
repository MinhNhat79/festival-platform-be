using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class MenuItemIngredient
    {
        [Key]
        public int ItemIngredientId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public string Unit { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
