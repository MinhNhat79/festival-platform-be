using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class AccountFestivalWalletCreateRequest
    {
        public int AccountId { get; set; }
        public int FestivalId { get; set; }
        public string? Name { get; set; }
        public decimal Balance { get; set; }
    }
}
