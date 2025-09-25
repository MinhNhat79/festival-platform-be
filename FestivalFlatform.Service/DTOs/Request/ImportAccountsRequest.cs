using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class ImportAccountsRequest
    {
        public int SchoolId { get; set; }
        public IFormFile ExcelFile { get; set; } = default!;
    }
}
