using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class IngredientCreateRequest
    {
        public int SupplierId { get; set; }
        public string IngredientName { get; set; } = null!;
        public string? Description { get; set; }
        public string Unit { get; set; } = null!;
        public decimal PricePerUnit { get; set; }
    }
}
