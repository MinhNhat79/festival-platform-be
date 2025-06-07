using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class ChatAttachment
    {
        [Key]
        public int AttachmentId { get; set; }

        [Required]
        public int MessageId { get; set; }

        [Required]
        public string AttachmentType { get; set; } = null!; // user_upload, ai_generated

        [Required]
        public string FileType { get; set; } = null!; // image, document

        public string? FileName { get; set; }

        [Required]
        public string FileUrl { get; set; } = null!;

        public int? FileSize { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
