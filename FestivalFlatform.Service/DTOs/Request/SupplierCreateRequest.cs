using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class SupplierCreateRequest
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(255)]
        public string CompanyName { get; set; } = null!;

        public string? BusinessLicense { get; set; }

        public string? Category { get; set; }

        public string? Note { get; set; }

        public string? Address { get; set; }

        public string? ContactInfo { get; set; }
    }
}
