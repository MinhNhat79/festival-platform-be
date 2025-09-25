using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        public string? ImageName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int? MenuItemId { get; set; }
        public int? BoothId { get; set; }
        public int? FestivalId { get; set; }
        public int? BoothMenuItemId { get; set; }
        public virtual BoothMenuItem? BoothMenuItem { get; set; }
        public virtual MenuItem? MenuItem { get; set; }
        public virtual Booth? Booth { get; set; }
        public virtual Festival? Festival { get; set; }


    }
}
