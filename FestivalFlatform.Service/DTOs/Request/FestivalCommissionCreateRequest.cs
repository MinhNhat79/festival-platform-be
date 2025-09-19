using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{

    public class FestivalCommissionCreateRequest
    {
        public int FestivalId { get; set; }

     
        public decimal Amount { get; set; }

        public double CommissionRate { get; set; }
    }
}
