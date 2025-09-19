using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Response
{
    public class AdminSummaryResponse
    {
        public int Schools { get; set; }
        public int FestivalsOngoing { get; set; }
        public decimal Gmv { get; set; }           
        public int PaidOrders { get; set; }
        public decimal Aov { get; set; }         
        public int BoothsActive { get; set; }
        public int UsersActive { get; set; }       
        public decimal WalletTopup { get; set; }   
        public decimal Commission { get; set; }    
    }

    // PaymentMixDto.cs
    public class PaymentMixResponse
    {
        public string Method { get; set; } = null!;
        public int Count { get; set; }
    }

    // TopFestivalDto.cs
    public class TopFestivalResponse
    {
        public int FestivalId { get; set; }
        public string FestivalName { get; set; } = null!;
        public int SchoolId { get; set; }
        public string SchoolName { get; set; } = null!;
     
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }


    public class AlertDto
    {
        public string Type { get; set; } = null!;
        public int Count { get; set; }
        public string Message { get; set; } = null!;
        public int? FestivalId { get; set; }
    }
}
