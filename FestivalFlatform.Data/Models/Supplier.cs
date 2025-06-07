using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public string CompanyName { get; set; } = null!;

        public string? BusinessLicense { get; set; }
        public string? Category { get; set; }
        public string Status { get; set; } = "pending";
        public string? Note { get; set; }
        public string? Address { get; set; }
        public string? ContactInfo { get; set; }
        public decimal? Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    }
}
