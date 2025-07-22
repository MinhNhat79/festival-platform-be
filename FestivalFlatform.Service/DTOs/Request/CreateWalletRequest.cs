using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreateWalletRequest
    {
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
    }
}
