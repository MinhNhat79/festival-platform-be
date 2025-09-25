using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class CommentCreateRequest
    {
        public int ReviewId { get; set; }
        public int AccountId { get; set; }
        public string Content { get; set; } = null!;
        public int? ParentCommentId { get; set; }
    }
}
