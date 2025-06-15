using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class FestivalIngredientCreateRequest
    {
        public int FestivalId { get; set; }
        public int IngredientId { get; set; }
        public int QuantityAvailable { get; set; }
        public decimal? SpecialPrice { get; set; }
    }

}
