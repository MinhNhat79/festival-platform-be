using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreateOrderRequest
    {
        public int AccountId { get; set; }
        public int BoothId { get; set; }
        public decimal TotalAmount { get; set; }
        public int PointsUsed { get; set; }
        public decimal CashAmount { get; set; }
        public string? Notes { get; set; }
    }
}
