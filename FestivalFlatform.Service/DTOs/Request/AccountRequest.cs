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

}
