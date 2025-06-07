using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class SchoolAccount
    {
        [Key]
        public int SchoolAccountId { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        public int AccountId { get; set; }

        public string? Position { get; set; }
        public string? Department { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public School? School { get; set; }
        public Account? Account { get; set; }
    }
}
