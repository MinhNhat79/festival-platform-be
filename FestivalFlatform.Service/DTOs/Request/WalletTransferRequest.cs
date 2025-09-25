using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class WalletTransferRequest
    {
        public int WalletId { get; set; }
        public int AccountFestivalWalletId { get; set; }
        public decimal Amount { get; set; }
    }
}
