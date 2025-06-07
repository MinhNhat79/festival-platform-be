using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class MenuItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public int BoothId { get; set; }

        [Required]
        public string ItemName { get; set; } = null!;

        public string? Description { get; set; }
        public decimal? QuantityLimit { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    }
}
