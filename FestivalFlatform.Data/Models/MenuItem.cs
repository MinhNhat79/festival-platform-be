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
        public int MenuId { get; set; }

        [Required]
        public string ItemName { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public string ItemType { get; set; } = null!; 


        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }


        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();
        public virtual ICollection<BoothMenuItem> BoothMenuItems { get; set; } = new List<BoothMenuItem>();

        public virtual FestivalMenu FestivalMenu { get; set; } = null!; 

    }
}
