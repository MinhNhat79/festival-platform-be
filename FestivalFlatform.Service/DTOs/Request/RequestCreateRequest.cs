using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class RequestCreateRequest
    {
        public int AccountId { get; set; }      
        public string Type { get; set; }       
        public string status { get; set; }       
        public string? Message { get; set; }   
        public string? Data { get; set; }   
    }
}
