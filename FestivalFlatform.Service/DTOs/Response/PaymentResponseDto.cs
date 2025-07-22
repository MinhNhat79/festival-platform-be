using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Response
{
    public class PaymentResponseDto
    {
        public object OrderCode { get; set; }
        public int PaymentId { get; set; }
        public int? OrderId { get; set; }
        public int? WalletId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentType { get; set; } = null!;
        public decimal AmountPaid { get; set; }
        public string Status { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime PaymentDate { get; set; }
        public string CheckoutUrl { get; set; } = null!;
    }
}
