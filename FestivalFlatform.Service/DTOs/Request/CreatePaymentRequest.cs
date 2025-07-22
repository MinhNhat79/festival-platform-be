using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreatePaymentRequest
    {
        public int? OrderId { get; set; }
        public int? WalletId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentType { get; set; } = null!;
        public decimal AmountPaid { get; set; }
        public string? Description { get; set; }
    }
}
