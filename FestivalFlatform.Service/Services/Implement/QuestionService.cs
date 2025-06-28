using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public QuestionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Question> CreateQuestionAsync(QuestionCreateRequest request)
        {
            var gameExists = await _unitOfWork.Repository<Minigame>().AnyAsync(g => g.GameId == request.GameId);
            if (!gameExists)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Minigame", request.GameId.ToString());

            var newQuestion = new Question
            {
                GameId = request.GameId,
                QuestionText = request.QuestionText.Trim(),
                Options = request.Options.Trim(),
                CorrectAnswer = request.CorrectAnswer.Trim(),
                Points = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Question>().InsertAsync(newQuestion);
            await _unitOfWork.CommitAsync();
            return newQuestion;
        }
        public async Task<Question> UpdateQuestionAsync(
        int questionId,
        int gameId,
        string questionText,
        string options,
        string correctAnswer,
        int points)
        {
            var question = await _unitOfWork.Repository<Question>().GetAll()
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);

            if (question == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Question", questionId.ToString());

            question.GameId = gameId;
            question.QuestionText = questionText.Trim();
            question.Options = options.Trim();
            question.CorrectAnswer = correctAnswer.Trim();
            question.Points = points;
            question.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return question;
        }
        public async Task<List<Question>> SearchQuestionsAsync(
        int? questionId, int? gameId, string? questionText, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Question>().GetAll()
                .Where(q => !questionId.HasValue || q.QuestionId == questionId.Value)
                .Where(q => !gameId.HasValue || q.GameId == gameId.Value)
                .Where(q => string.IsNullOrWhiteSpace(questionText) || q.QuestionText.Contains(questionText.Trim()));

            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);

            return await query.Skip((currentPage - 1) * currentSize).Take(currentSize).ToListAsync();
        }
        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _unitOfWork.Repository<Question>().FindAsync(q => q.QuestionId == questionId);
            if (question == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Question", questionId.ToString());

            _unitOfWork.Repository<Question>().Delete(question);
            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
