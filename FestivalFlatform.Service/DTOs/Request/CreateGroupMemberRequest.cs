using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CreateGroupMemberRequest
    {
        public int GroupId { get; set; }
        public int AccountId { get; set; }
        public string? Role { get; set; }
    }
}
