using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class FestivalParticipantCreateRequest
    {
        public int FestivalId { get; set; }
        public int AccountId { get; set; }
    }
}
