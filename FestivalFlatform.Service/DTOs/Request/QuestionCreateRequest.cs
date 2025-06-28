using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class QuestionCreateRequest
    {
        public int GameId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string Options { get; set; } = null!;
        public string CorrectAnswer { get; set; } = null!;

    }
}
