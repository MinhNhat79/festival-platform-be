using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Response
{
    public class AccountResponse
    {
        public int Id {  get; set; }  
        public string? Email { get; set; }

        public string? Pasword { get; set; } 
        public string? FullName { get; set; } 
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } 

    }
}
