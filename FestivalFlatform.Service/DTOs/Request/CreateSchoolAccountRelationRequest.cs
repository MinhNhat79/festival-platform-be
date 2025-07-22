using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreateSchoolAccountRelationRequest
    {
        public int SchoolId { get; set; }
        public int AccountId { get; set; }
        public string RelationType { get; set; } = null!;
    }
}
