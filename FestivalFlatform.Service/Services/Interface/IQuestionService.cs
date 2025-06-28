using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IQuestionService
    {
        Task<Question> CreateQuestionAsync(QuestionCreateRequest request);
        Task<Question> UpdateQuestionAsync(
        int questionId,
        int gameId,
        string questionText,
        string options,
        string correctAnswer,
        int points);
        Task<List<Question>> SearchQuestionsAsync(
        int? questionId, int? gameId, string? questionText, int? pageNumber, int? pageSize);
        Task<bool> DeleteQuestionAsync(int questionId);
    }
}
