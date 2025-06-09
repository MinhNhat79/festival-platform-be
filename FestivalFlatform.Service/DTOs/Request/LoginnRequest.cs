using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class LoginnRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class EmailValidateRequest
    {
        public string Email { get; set; }
    }
}
