using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreateFestivalWalletRequest
    {
        public int FestivalId { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
