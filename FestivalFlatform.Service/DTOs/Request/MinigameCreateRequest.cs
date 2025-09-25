using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class MinigameCreateRequest
    {
        public int BoothId { get; set; }
        public string GameName { get; set; } = null!;
       
        public string? Rules { get; set; }
        public int? RewardPoints { get; set; }

    }
}
