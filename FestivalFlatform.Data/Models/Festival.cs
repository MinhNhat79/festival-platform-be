using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Festival
    {
        [Key]
        public int FestivalId { get; set; }

        [Required]
        public int OrganizerSchoolId { get; set; }

        [Required]
        public string FestivalName { get; set; } = null!;

        public string? Theme { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public string? Location { get; set; }
        public int? MaxBooths { get; set; }

        public string Status { get; set; } = "draft";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Booth> Booths { get; set; } = new List<Booth>();
        public virtual ICollection<FestivalIngredient> FestivalIngredients { get; set; } = new List<FestivalIngredient>();
        public virtual ICollection<FestivalMap> FestivalMaps { get; set; } = new List<FestivalMap>();
        public virtual ICollection<FestivalSchool> FestivalSchools { get; set; } = new List<FestivalSchool>();
    }
}
