using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role? Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual AccountPoints? AccountPoints { get; set; }
        public virtual ICollection<SchoolAccount> SchoolAccounts { get; set; } = new List<SchoolAccount>();
        public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
        public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<PointsTransaction> PointsTransactions { get; set; } = new List<PointsTransaction>();
        public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        
    }
}
