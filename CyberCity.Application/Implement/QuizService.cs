using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Quizzes;
using CyberCity.Infrastructure;

namespace CyberCity.Application.Implement
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepo;
        private readonly IMapper _mapper;

        public QuizService(IQuizRepository quizRepo, IMapper mapper)
        {
            _quizRepo = quizRepo;
            _mapper = mapper;
        }

        public async Task<QuizDetailDto> GetQuizByIdAsync(string quizId, string studentId)
        {
            var quiz = await _quizRepo.GetByIdAsync(quizId);
            if (quiz == null)
            {
                throw new Exception("Quiz không tồn tại");
            }

            var questions = await _quizRepo.GetQuizQuestionsAsync(quizId);
            var questionsWithAnswers = new List<QuestionWithAnswersDto>();

            foreach (var question in questions)
            {
                var answers = await _quizRepo.GetQuestionAnswersAsync(question.Uid);
                
                questionsWithAnswers.Add(new QuestionWithAnswersDto
                {
                    Question = _mapper.Map<QuizQuestionDto>(question),
                    Answers = _mapper.Map<List<QuizAnswerDto>>(answers)
                });
            }

            var userSubmission = await _quizRepo.GetUserSubmissionAsync(quizId, studentId);

            return new QuizDetailDto
            {
                Quiz = _mapper.Map<QuizDto>(quiz),
                Questions = questionsWithAnswers,
                UserSubmission = _mapper.Map<QuizSubmissionDto>(userSubmission)
            };
        }

        public async Task<SubmitQuizResponseDto> SubmitQuizAsync(string quizId, string studentId, SubmitQuizDto submitDto)
        {
            var quiz = await _quizRepo.GetByIdAsync(quizId);
            if (quiz == null)
            {
                throw new Exception("Quiz không tồn tại");
            }

            // Kiểm tra xem user đã submit chưa
            var existingSubmission = await _quizRepo.GetUserSubmissionAsync(quizId, studentId);
            if (existingSubmission != null)
            {
                throw new Exception("Bạn đã nộp bài quiz này rồi");
            }

            // Chấm điểm
            var questions = await _quizRepo.GetQuizQuestionsAsync(quizId);
            int correctCount = 0;
            var details = new List<QuizResultDetailDto>();

            foreach (var answer in submitDto.Answers)
            {
                // Get all answers for this question
                var questionAnswers = await _quizRepo.GetQuestionAnswersAsync(answer.QuestionId);
                var selectedAnswer = questionAnswers.FirstOrDefault(qa => qa.Uid == answer.SelectedAnswerId);
                var correctAnswerId = questionAnswers.FirstOrDefault(a => a.IsCorrect == true)?.Uid;

                bool isCorrect = selectedAnswer?.IsCorrect == true;
                if (isCorrect) correctCount++;

                details.Add(new QuizResultDetailDto
                {
                    QuestionId = answer.QuestionId,
                    IsCorrect = isCorrect,
                    CorrectAnswerId = correctAnswerId
                });
            }

            decimal score = ((decimal)correctCount / questions.Count) * 100;

            // Lưu submission
            var submission = new QuizSubmission
            {
                Uid = Guid.NewGuid().ToString(),
                QuizUid = quizId.ToString(),
                StudentUid = studentId.ToString(),
                Score = score,
                SubmittedAt = DateTime.Now
            };

            await _quizRepo.CreateSubmissionAsync(submission);

            // Lưu chi tiết câu trả lời
            var submissionAnswers = submitDto.Answers.Select(a => new QuizSubmissionAnswer
            {
                Uid = Guid.NewGuid().ToString(),
                SubmissionUid = submission.Uid,
                QuestionUid = a.QuestionId,
                SelectedAnswerUid = a.SelectedAnswerId,
                IsCorrect = details.FirstOrDefault(d => d.QuestionId == a.QuestionId)?.IsCorrect ?? false
            }).ToList();

            await _quizRepo.CreateSubmissionAnswersAsync(submissionAnswers);

            return new SubmitQuizResponseDto
            {
                Submission = _mapper.Map<QuizSubmissionDto>(submission),
                Score = score,
                TotalQuestions = questions.Count,
                CorrectAnswers = correctCount,
                Details = details
            };
        }
    }
}

