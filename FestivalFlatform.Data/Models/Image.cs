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

        public int? MenuItemId { get; set; } // Sửa thành nullable

        [Required]
        public string ImageUrl { get; set; } = null!;

        public string? ImageName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(MenuItemId))]
        public virtual MenuItem? MenuItem { get; set; } // Nullable luôn cho đồng bộ
    }
}
