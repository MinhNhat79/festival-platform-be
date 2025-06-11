using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class StudentGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public string ClassName { get; set; } = null!;

        [Required]
        public string GroupName { get; set; } = null!;

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public decimal GroupBudget { get; set; } = 0;
        public string Status { get; set; } = "active";

        public DateTime? UpdatedAt { get; set; }


        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public virtual ICollection<Booth> Booths { get; set; } = new List<Booth>();
    }
}
