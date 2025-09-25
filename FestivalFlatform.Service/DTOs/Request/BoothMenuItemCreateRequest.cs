using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class BoothMenuItemCreateRequest
    {
        public int BoothId { get; set; }
        public int MenuItemId { get; set; }
        //public decimal? CustomPrice { get; set; }
        public int? QuantityLimit { get; set; }
        public decimal ?CustomPrice { get; set; }
    }
}
