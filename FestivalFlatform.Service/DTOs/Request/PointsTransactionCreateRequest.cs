using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class PointsTransactionCreateRequest
    {
        public int AccountId { get; set; }
        public int? BoothId { get; set; }
        public int? GameId { get; set; }
        public int PointsAmount { get; set; }
        public string TransactionType { get; set; } = null!;
    }
}
