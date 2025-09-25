using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

       
        public int? OrderId { get; set; }

        public int? WalletId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = null!; 

        
        public string PaymentType { get; set; } = null!;

      
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    
        public decimal AmountPaid { get; set; }


        public string Status { get; set; } = "pending";

        public string? Description { get; set; }

        public virtual Order? Order { get; set; } 
        public virtual Wallet? Wallet { get; set; }
    }
}
