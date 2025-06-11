using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class School
    {
        [Key]
        public int SchoolId { get; set; }

        [Required]
        public string SchoolName { get; set; } = null!;

        public string? Address { get; set; }
        public string? ContactInfo { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<SchoolAccount> SchoolAccounts { get; set; } = new List<SchoolAccount>();
        public virtual ICollection<Festival> Festivals { get; set; } = new List<Festival>();
        public virtual ICollection<FestivalSchool> FestivalSchools { get; set; } = new List<FestivalSchool>();
    }
}
