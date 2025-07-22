using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class SchoolAccountRelation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RelationType { get; set; } = null!; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual School School { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
    }
}
