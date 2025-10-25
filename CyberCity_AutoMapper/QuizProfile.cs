using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Quizzes;

namespace CyberCity.AutoMapper
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<Quiz, QuizDto>();
            CreateMap<QuizDto, Quiz>();

            CreateMap<QuizQuestion, QuizQuestionDto>();
            CreateMap<QuizQuestionDto, QuizQuestion>();

            CreateMap<QuizAnswer, QuizAnswerDto>();
            CreateMap<QuizAnswerDto, QuizAnswer>();

            CreateMap<QuizSubmission, QuizSubmissionDto>();
            CreateMap<QuizSubmissionDto, QuizSubmission>();

            CreateMap<QuizSubmissionAnswer, QuizSubmissionAnswerDto>();
            CreateMap<QuizSubmissionAnswerDto, QuizSubmissionAnswer>();
        }
    }
}

