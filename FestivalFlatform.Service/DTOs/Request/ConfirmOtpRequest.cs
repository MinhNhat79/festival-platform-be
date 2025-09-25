using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class ConfirmOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
