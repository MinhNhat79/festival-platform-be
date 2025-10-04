using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    [Table("Request")]
    public class Request
    {
        public int Id { get; set; }

        public string Status { get; set; } 

        public string? Message { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;

        public string? Type { get; set; }
        public string? Data { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
