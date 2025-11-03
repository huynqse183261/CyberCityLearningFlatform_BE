using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Learning;
using CyberCity.DTOs.Answers;
using CyberCity.DTOs.Courses;
using CyberCity.DTOs.Lessons;
using CyberCity.DTOs.Modules;
using CyberCity.DTOs.Quizzes;
using CyberCity.DTOs.Topics;
using CyberCity.DTOs.Enrollments;
using CyberCity.DTOs.Labs;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CyberCity.Application.Implement
{
    public class LearningService : ILearningService
    {
        private readonly CourseRepo _courseRepo;
        private readonly ModuleRepo _moduleRepo;
        private readonly LessonRepo _lessonRepo;
        private readonly TopicRepo _topicRepo;
        private readonly SubtopicRepo _subtopicRepo;
        private readonly IAnswerRepository _answerRepo;
        private readonly IQuizRepository _quizRepo;
        private readonly ILabRepository _labRepo;
        private readonly CourseEnrollmentRepo _enrollmentRepo;
        private readonly IMapper _mapper;

        public LearningService(
            CourseRepo courseRepo,
            ModuleRepo moduleRepo,
            LessonRepo lessonRepo,
            TopicRepo topicRepo,
            SubtopicRepo subtopicRepo,
            IAnswerRepository answerRepo,
            IQuizRepository quizRepo,
            ILabRepository labRepo,
            CourseEnrollmentRepo enrollmentRepo,
            IMapper mapper)
        {
            _courseRepo = courseRepo;
            _moduleRepo = moduleRepo;
            _lessonRepo = lessonRepo;
            _topicRepo = topicRepo;
            _subtopicRepo = subtopicRepo;
            _answerRepo = answerRepo;
            _quizRepo = quizRepo;
            _labRepo = labRepo;
            _enrollmentRepo = enrollmentRepo;
            _mapper = mapper;
        }

        public async Task<List<CourseDto>> GetAllCoursesAsync(string? studentId = null)
        {
            var courses = await _courseRepo.GetAllAsync().ToListAsync();
            return _mapper.Map<List<CourseDto>>(courses);
        }

        public async Task<CourseDetailDto> GetCourseDetailAsync(string courseId, string studentId)
        {
            var course = await _courseRepo.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new Exception("Khóa học không tồn tại");
            }

            var modules = await _moduleRepo.GetAllAsync()
                .Where(m => m.CourseUid == courseId.ToString())
                .OrderBy(m => m.OrderIndex)
                .ToListAsync();

            var allEnrollments = await _enrollmentRepo.GetAllAsync();
            var enrollment = allEnrollments
                .FirstOrDefault(e => e.CourseUid == courseId.ToString() && e.UserUid == studentId.ToString());

            // Tính tiến độ
            var totalLessons = 0;
            var completedLessons = 0;

            var allLessons = await _lessonRepo.GetAllAsync().ToListAsync();
            var allTopics = await _topicRepo.GetAllAsync().ToListAsync();
            var allSubtopics = await _subtopicRepo.GetAllAsync().ToListAsync();

            foreach (var module in modules)
            {
                var lessons = allLessons
                    .Where(l => l.ModuleUid == module.Uid)
                    .ToList();
                
                totalLessons += lessons.Count;

                foreach (var lesson in lessons)
                {
                    var topics = allTopics
                        .Where(t => t.LessonUid == lesson.Uid)
                        .ToList();

                    var subtopicCount = 0;
                    var completedSubtopicCount = 0;

                    foreach (var topic in topics)
                    {
                        var subtopics = allSubtopics
                            .Where(st => st.TopicUid == topic.Uid)
                            .ToList();
                        
                        subtopicCount += subtopics.Count;

                        foreach (var subtopic in subtopics)
                        {
                            var progress = await _answerRepo.GetUserProgressAsync(studentId, subtopic.Uid);
                            if (progress != null && progress.IsCompleted == true)
                            {
                                completedSubtopicCount++;
                            }
                        }
                    }

                    if (subtopicCount > 0 && completedSubtopicCount == subtopicCount)
                    {
                        completedLessons++;
                    }
                }
            }

            var progressPercentage = totalLessons > 0 ? ((decimal)completedLessons / totalLessons) * 100 : 0;

            return new CourseDetailDto
            {
                Course = _mapper.Map<CourseDto>(course),
                Modules = _mapper.Map<List<ModuleDto>>(modules),
                Enrollment = _mapper.Map<CourseEnrollmentDto>(enrollment),
                Progress = new CourseProgressSummaryDto
                {
                    TotalLessons = totalLessons,
                    CompletedLessons = completedLessons,
                    Percentage = progressPercentage
                }
            };
        }

        public async Task<ModuleDetailResponseDto> GetModuleDetailAsync(string moduleId, string studentId)
        {
            var module = await _moduleRepo.GetByIdAsync(moduleId);
            if (module == null)
            {
                throw new Exception("Module không tồn tại");
            }

            var lessons = await _lessonRepo.GetAllAsync()
                .Where(l => l.ModuleUid == moduleId)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync();

            var labs = await _labRepo.GetLabsByModuleIdAsync(moduleId);

            var totalItems = lessons.Count + labs.Count;
            var completedItems = 0;

            var allTopics = await _topicRepo.GetAllAsync().ToListAsync();
            var allSubtopics = await _subtopicRepo.GetAllAsync().ToListAsync();

            // Đếm lessons đã hoàn thành
            foreach (var lesson in lessons)
            {
                var topics = allTopics
                    .Where(t => t.LessonUid == lesson.Uid)
                    .ToList();

                var subtopicCount = 0;
                var completedSubtopicCount = 0;

                foreach (var topic in topics)
                {
                    var subtopics = allSubtopics
                        .Where(st => st.TopicUid == topic.Uid)
                        .ToList();
                    
                    subtopicCount += subtopics.Count;

                    foreach (var subtopic in subtopics)
                    {
                        var progress = await _answerRepo.GetUserProgressAsync(studentId, subtopic.Uid);
                        if (progress != null && progress.IsCompleted == true)
                        {
                            completedSubtopicCount++;
                        }
                    }
                }

                if (subtopicCount > 0 && completedSubtopicCount == subtopicCount)
                {
                    completedItems++;
                }
            }

            var progressPercentage = totalItems > 0 ? ((decimal)completedItems / totalItems) * 100 : 0;

            return new ModuleDetailResponseDto
            {
                Module = _mapper.Map<ModuleDto>(module),
                Lessons = _mapper.Map<List<LessonDto>>(lessons),
                Labs = _mapper.Map<List<LabDto>>(labs),
                Progress = new ModuleProgressDto
                {
                    TotalItems = totalItems,
                    CompletedItems = completedItems,
                    Percentage = progressPercentage
                }
            };
        }

        public async Task<LearningContentDto> GetLessonContentAsync(string lessonId, string studentId)
        {
            var lesson = await _lessonRepo.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                throw new Exception("Bài học không tồn tại");
            }

            var topics = await _topicRepo.GetAllAsync()
                .Where(t => t.LessonUid == lessonId)
                .OrderBy(t => t.OrderIndex)
                .ToListAsync();

            var allSubtopics = await _subtopicRepo.GetAllAsync().ToListAsync();
            var contentList = new List<TopicWithSubtopicsDto>();

            foreach (var topic in topics)
            {
                var subtopics = allSubtopics
                    .Where(st => st.TopicUid == topic.Uid)
                    .OrderBy(st => st.OrderIndex)
                    .ToList();

                var subtopicsWithProgress = new List<SubtopicWithProgressDto>();

                foreach (var subtopic in subtopics)
                {
                    var progress = await _answerRepo.GetUserProgressAsync(studentId, subtopic.Uid);
                    var answer = await _answerRepo.GetAnswerBySubtopicIdAsync(subtopic.Uid);

                    subtopicsWithProgress.Add(new SubtopicWithProgressDto
                    {
                        Uid = subtopic.Uid,
                        TopicUid = subtopic.TopicUid,
                        Title = subtopic.Title,
                        Content = subtopic.Content,
                        OrderIndex = subtopic.OrderIndex ?? 0,
                        IsCompleted = progress?.IsCompleted == true,
                        HasAnswer = answer != null,
                        CreatedAt = subtopic.CreatedAt
                    });
                }

                contentList.Add(new TopicWithSubtopicsDto
                {
                    Topic = _mapper.Map<TopicDto>(topic),
                    Subtopics = subtopicsWithProgress
                });
            }

            var quiz = await _quizRepo.GetQuizByLessonIdAsync(lessonId);

            // Navigation
            var allModules = await _moduleRepo.GetAllAsync().ToListAsync();
            var allLessonsForNav = await _lessonRepo.GetAllAsync().ToListAsync();
            
            var module = allModules
                .FirstOrDefault(m => allLessonsForNav.Any(l => l.Uid == lessonId.ToString() && l.ModuleUid == m.Uid));

            var lessonsInModule = module != null 
                ? allLessonsForNav
                    .Where(l => l.ModuleUid == module.Uid)
                    .OrderBy(l => l.OrderIndex)
                    .ToList()
                : new List<Lesson>();

            var currentIndex = lessonsInModule.FindIndex(l => l.Uid == lessonId.ToString());
            var prevLesson = currentIndex > 0 ? lessonsInModule[currentIndex - 1] : null;
            var nextLesson = currentIndex < lessonsInModule.Count - 1 ? lessonsInModule[currentIndex + 1] : null;

            return new LearningContentDto
            {
                Lesson = _mapper.Map<LessonDto>(lesson),
                Content = contentList,
                Quiz = _mapper.Map<QuizDto>(quiz),
                Navigation = new NavigationDto
                {
                    PrevLesson = prevLesson != null ? new NavigationItemDto { Id = prevLesson.Uid, Title = prevLesson.Title } : null,
                    NextLesson = nextLesson != null ? new NavigationItemDto { Id = nextLesson.Uid, Title = nextLesson.Title } : null,
                    CurrentModule = module != null ? new NavigationItemDto { Id = module.Uid, Title = module.Title } : null
                }
            };
        }

        public async Task<SubmitAnswerResponseDto> SubmitSubtopicAnswerAsync(string subtopicId, string studentId, SubmitAnswerDto submitDto)
        {
            var subtopic = await _subtopicRepo.GetByIdAsync(subtopicId);
            if (subtopic == null)
            {
                throw new Exception("Subtopic không tồn tại");
            }

            var answer = await _answerRepo.GetAnswerBySubtopicIdAsync(subtopicId);
            if (answer == null)
            {
                throw new Exception("Subtopic này không có câu trả lời để kiểm tra");
            }

            // Kiểm tra câu trả lời
            bool isCorrect = CheckAnswer(submitDto.UserOutput, answer);

            // Lấy hoặc tạo progress
            var progress = await _answerRepo.GetUserProgressAsync(studentId, subtopicId);
            if (progress == null)
            {
                progress = new SubtopicProgress
                {
                    Uid = Guid.NewGuid().ToString(),
                    StudentUid = studentId.ToString(),
                    SubtopicUid = subtopicId.ToString(),
                    IsCompleted = isCorrect,
                    CompletedAt = isCorrect ? DateTime.Now : null,
                    UserOutput = submitDto.UserOutput,
                    IsCorrect = isCorrect,
                    AttemptCount = 1,
                    LastAttemptedAt = DateTime.Now
                };
            }
            else
            {
                progress.UserOutput = submitDto.UserOutput;
                progress.IsCorrect = isCorrect;
                progress.AttemptCount += 1;
                progress.LastAttemptedAt = DateTime.Now;
                
                if (isCorrect && (progress.IsCompleted != true))
                {
                    progress.IsCompleted = true;
                    progress.CompletedAt = DateTime.Now;
                }
            }

            await _answerRepo.CreateOrUpdateProgressAsync(progress);

            return new SubmitAnswerResponseDto
            {
                IsCorrect = isCorrect,
                ExpectedOutput = answer.ExpectedOutput,
                Explanation = answer.Explanation,
                Hints = isCorrect ? null : answer.Hints,
                Progress = _mapper.Map<SubtopicProgressDto>(progress)
            };
        }

        public async Task<SubtopicProgressDto> CompleteSubtopicAsync(string subtopicId, string studentId)
        {
            var subtopic = await _subtopicRepo.GetByIdAsync(subtopicId);
            if (subtopic == null)
            {
                throw new Exception("Subtopic không tồn tại");
            }

            var progress = await _answerRepo.GetUserProgressAsync(studentId, subtopicId);
            if (progress == null)
            {
                progress = new SubtopicProgress
                {
                    Uid = Guid.NewGuid().ToString(),
                    StudentUid = studentId.ToString(),
                    SubtopicUid = subtopicId.ToString(),
                    IsCompleted = true,
                    CompletedAt = DateTime.Now,
                    UserOutput = null,
                    IsCorrect = true,
                    AttemptCount = 0,
                    LastAttemptedAt = null
                };
            }
            else if (progress.IsCompleted != true)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.Now;
            }

            await _answerRepo.CreateOrUpdateProgressAsync(progress);
            return _mapper.Map<SubtopicProgressDto>(progress);
        }

        public async Task<StudentProgressDto> GetStudentProgressAsync(string studentId)
        {
            var allEnrollments = await _enrollmentRepo.GetAllAsync();
            var enrollments = allEnrollments
                .Where(e => e.UserUid == studentId)
                .ToList();

            var totalCourses = await _courseRepo.GetAllCourseAsync().CountAsync();
            var enrolledCourses = enrollments.Count;

            var totalLessons = 0;
            var completedLessons = 0;
            var totalQuizzes = 0;
            var completedQuizzes = 0;
            var totalLabs = 0;

            var allModules = await _moduleRepo.GetAllAsync().ToListAsync();
            var allLessons = await _lessonRepo.GetAllAsync().ToListAsync();

            foreach (var enrollment in enrollments)
            {
                var modules = allModules
                    .Where(m => m.CourseUid == enrollment.CourseUid)
                    .ToList();

                foreach (var module in modules)
                {
                    var lessons = allLessons
                        .Where(l => l.ModuleUid == module.Uid)
                        .ToList();

                    totalLessons += lessons.Count;

                    var labs = await _labRepo.GetLabsByModuleIdAsync(module.Uid);
                    totalLabs += labs.Count;

                    foreach (var lesson in lessons)
                    {
                        // Check lesson completion
                        var topics = await _topicRepo.GetAllAsync()
                            .Where(t => t.LessonUid == lesson.Uid)
                            .ToListAsync();

                        var subtopicCount = 0;
                        var completedSubtopicCount = 0;

                        foreach (var topic in topics)
                        {
                            var subtopics = await _subtopicRepo.GetAllAsync()
                                .Where(st => st.TopicUid == topic.Uid)
                                .ToListAsync();
                            
                            subtopicCount += subtopics.Count;

                            foreach (var subtopic in subtopics)
                            {
                                var progress = await _answerRepo.GetUserProgressAsync(studentId, subtopic.Uid);
                                if (progress != null && progress.IsCompleted == true)
                                {
                                    completedSubtopicCount++;
                                }
                            }
                        }

                        if (subtopicCount > 0 && completedSubtopicCount == subtopicCount)
                        {
                            completedLessons++;
                        }

                        // Check quiz completion
                        var quiz = await _quizRepo.GetQuizByLessonIdAsync(lesson.Uid);
                        if (quiz != null)
                        {
                            totalQuizzes++;
                            var submission = await _quizRepo.GetUserSubmissionAsync(quiz.Uid, studentId);
                            if (submission != null)
                            {
                                completedQuizzes++;
                            }
                        }
                    }
                }
            }

            var overallProgress = totalLessons > 0 ? ((decimal)completedLessons / totalLessons) * 100 : 0;

            return new StudentProgressDto
            {
                TotalCourses = totalCourses,
                EnrolledCourses = enrolledCourses,
                CompletedCourses = 0, // TODO: Calculate based on course completion criteria
                TotalLessons = totalLessons,
                CompletedLessons = completedLessons,
                TotalQuizzes = totalQuizzes,
                CompletedQuizzes = completedQuizzes,
                TotalLabs = totalLabs,
                CompletedLabs = 0, // TODO: Need LabProgress table
                OverallProgress = overallProgress
            };
        }

        public async Task<CourseProgressDetailDto> GetCourseProgressDetailAsync(string courseId, string studentId)
        {
            var course = await _courseRepo.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new Exception("Khóa học không tồn tại");
            }

            var modules = await _moduleRepo.GetAllAsync()
                .Where(m => m.CourseUid == courseId)
                .OrderBy(m => m.OrderIndex)
                .ToListAsync();

            var allLessons = await _lessonRepo.GetAllAsync().ToListAsync();
            var allTopics = await _topicRepo.GetAllAsync().ToListAsync();
            var allSubtopics = await _subtopicRepo.GetAllAsync().ToListAsync();

            var moduleProgressList = new List<ModuleProgressDetailDto>();
            var totalModuleProgress = 0m;

            foreach (var module in modules)
            {
                var lessons = allLessons
                    .Where(l => l.ModuleUid == module.Uid)
                    .OrderBy(l => l.OrderIndex)
                    .ToList();

                var labs = await _labRepo.GetLabsByModuleIdAsync(module.Uid);

                var lessonsProgressList = new List<LessonProgressDto>();

                foreach (var lesson in lessons)
                {
                    var topics = allTopics
                        .Where(t => t.LessonUid == lesson.Uid)
                        .ToList();

                    var subtopicsTotal = 0;
                    var subtopicsCompleted = 0;

                    foreach (var topic in topics)
                    {
                        var subtopics = allSubtopics
                            .Where(st => st.TopicUid == topic.Uid)
                            .ToList();
                        
                        subtopicsTotal += subtopics.Count;

                        foreach (var subtopic in subtopics)
                        {
                            var progress = await _answerRepo.GetUserProgressAsync(studentId, subtopic.Uid);
                            if (progress != null && progress.IsCompleted == true)
                            {
                                subtopicsCompleted++;
                            }
                        }
                    }

                    var quiz = await _quizRepo.GetQuizByLessonIdAsync(lesson.Uid);
                    decimal? quizScore = null;
                    if (quiz != null)
                    {
                        var submission = await _quizRepo.GetUserSubmissionAsync(quiz.Uid, studentId);
                        quizScore = submission?.Score;
                    }

                    lessonsProgressList.Add(new LessonProgressDto
                    {
                        Lesson = new LessonProgressInfoDto
                        {
                            Uid = lesson.Uid,
                            Title = lesson.Title,
                            OrderIndex = lesson.OrderIndex ?? 0
                        },
                        SubtopicsCompleted = subtopicsCompleted,
                        SubtopicsTotal = subtopicsTotal,
                        QuizScore = quizScore
                    });
                }

                var labsProgressList = labs.Select(lab => new LabProgressDto
                {
                    Lab = new LabProgressInfoDto
                    {
                        Uid = lab.Uid,
                        Title = lab.Title,
                        IsRequired = lab.IsRequired == true
                    },
                    IsCompleted = false // TODO: Need LabProgress table
                }).ToList();

                var completedLessons = lessonsProgressList.Count(lp => lp.SubtopicsTotal > 0 && lp.SubtopicsCompleted == lp.SubtopicsTotal);
                var totalItems = lessons.Count + labs.Count;
                var moduleProgress = totalItems > 0 ? ((decimal)completedLessons / totalItems) * 100 : 0;
                totalModuleProgress += moduleProgress;

                moduleProgressList.Add(new ModuleProgressDetailDto
                {
                    Module = new ModuleProgressInfoDto
                    {
                        Uid = module.Uid,
                        Title = module.Title,
                        OrderIndex = module.OrderIndex ?? 0
                    },
                    LessonsProgress = lessonsProgressList,
                    LabsProgress = labsProgressList,
                    ModuleProgress = moduleProgress
                });
            }

            var courseProgress = modules.Count > 0 ? totalModuleProgress / modules.Count : 0;

            return new CourseProgressDetailDto
            {
                Course = new CourseProgressInfoDto
                {
                    Uid = course.Uid,
                    Title = course.Title,
                    Description = course.Description,
                    Level = course.Level
                },
                Modules = moduleProgressList,
                CourseProgress = courseProgress
            };
        }

        private bool CheckAnswer(string userOutput, Answer answer)
        {
            if (string.IsNullOrWhiteSpace(userOutput))
            {
                return false;
            }

            var expectedOutput = answer.ExpectedOutput;

            if (answer.TrimWhitespace == true)
            {
                userOutput = userOutput.Trim();
                expectedOutput = expectedOutput.Trim();
            }

            return answer.CheckType.ToLower() switch
            {
                "exact" => answer.CaseSensitive == true
                    ? userOutput == expectedOutput 
                    : userOutput.Equals(expectedOutput, StringComparison.OrdinalIgnoreCase),
                    
                "contains" => answer.CaseSensitive == true
                    ? userOutput.Contains(expectedOutput) 
                    : userOutput.Contains(expectedOutput, StringComparison.OrdinalIgnoreCase),
                    
                "regex" => Regex.IsMatch(userOutput, expectedOutput),
                
                _ => false
            };
        }
    }
}

