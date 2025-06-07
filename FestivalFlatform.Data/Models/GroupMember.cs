using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class GroupMember
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int AccountId { get; set; }

        public string? Role { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    }
}
