using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class BoothApproveRequest
    {
        public DateTime ApprovalDate { get; set; }
        public int PointsBalance { get; set; }
    }
}
