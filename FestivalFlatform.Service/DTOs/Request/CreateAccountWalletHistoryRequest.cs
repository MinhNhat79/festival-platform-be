using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreateAccountWalletHistoryRequest
    {
        public int AccountId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } 
        public string? Type { get; set; }
    }
}
