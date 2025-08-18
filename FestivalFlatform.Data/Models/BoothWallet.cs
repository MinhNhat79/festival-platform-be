using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class BoothWallet
    {
        public int BoothWalletId { get; set; }

        public int BoothId { get; set; }

        public decimal TotalBalance { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Booth Booth { get; set; } = null!;
    }
}
