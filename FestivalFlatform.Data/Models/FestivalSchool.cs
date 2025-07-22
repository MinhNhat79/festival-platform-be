using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class FestivalSchool
    {
        [Key]
        public int FestivalSchoolId { get; set; }

        [Required]
        public int FestivalId { get; set; }

        [Required]
        public int SchoolId { get; set; }

        public string Status { get; set; } = "pending";

        public string? rejectReason { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovalDate { get; set; }

        public virtual Festival Festival { get; set; } = null!;

        public virtual School School { get; set; } = null!;
            
    }
}
