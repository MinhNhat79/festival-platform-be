using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class AccountRequest
    {

        public string? Email { get; set; } = null;

        public string? Pasword { get; set; } = null;
        public string? FullNme { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public DateTime? CreatedAt{ get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;
      
    }
    public class AccountUpdateRequest
    {
        public string? FullName  { get; set; } = null;
        public string? Email { get; set; } = null;

        public string? Pasword { get; set; } = null;
       public string? ClassName { get; set; } = null;   
        public string? PhoneNumber { get; set; } = null;
        public string? AvatarUrl { get; set; } = null;
        public bool? Status { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;

    }
    public class AccountSearchRequest
    {
        public int? AccountId { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
    }

}
