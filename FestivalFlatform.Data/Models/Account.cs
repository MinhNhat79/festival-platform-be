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


        public string? PlainPassword { get; set; }
        [Required]
        public string FullName { get; set; } = null!;

        public string? ClassName { get; set; }
        public string? PhoneNumber { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role? Role { get; set; }
        public string? AvatarUrl { get; set; }
        public string? OtpVerify { get; set; }

        public bool Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual AccountPoints? AccountPoints { get; set; }

        public virtual ICollection<School> Schools { get; set; } = new List<School>();
        public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
        public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<PointsTransaction> PointsTransactions { get; set; } = new List<PointsTransaction>();
        public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public virtual ICollection<SchoolAccountRelation> SchoolAccountRelations { get; set; } = new List<SchoolAccountRelation>();
        public virtual ICollection<AccountFestivalWallet> AccountFestivalWallets { get; set; } = new List<AccountFestivalWallet>();

        public virtual ICollection<AccountWalletHistory> WalletHistories { get; set; } = new List<AccountWalletHistory>();
        public virtual ICollection<FestivalParticipant> FestivalParticipants { get; set; } = new List<FestivalParticipant>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();


    }
}
