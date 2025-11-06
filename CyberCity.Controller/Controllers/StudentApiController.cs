using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace CyberCity.Controller.Controllers
{
    // Student-facing, FE-friendly endpoints following API_SPECIFICATION.md
    [ApiController]
    [Route("api/student")]
    [Authorize]
    public class StudentApiController : ControllerBase
    {
        private readonly CourseRepo _courseRepo;
        private readonly ModuleRepo _moduleRepo;
        private readonly LessonRepo _lessonRepo;
        private readonly TopicRepo _topicRepo;
        private readonly SubtopicRepo _subtopicRepo;
        private readonly IAnswerRepository _answerRepo;
        private readonly IQuizRepository _quizRepo;
        private readonly ICourseEnrollmentService _enrollmentService;
        private readonly ISubscriptionService _subscriptionService;

        public StudentApiController(
            CourseRepo courseRepo,
            ModuleRepo moduleRepo,
            LessonRepo lessonRepo,
            TopicRepo topicRepo,
            SubtopicRepo subtopicRepo,
            IAnswerRepository answerRepo,
            IQuizRepository quizRepo,
            ICourseEnrollmentService enrollmentService,
            ISubscriptionService subscriptionService)
        {
            _courseRepo = courseRepo;
            _moduleRepo = moduleRepo;
            _lessonRepo = lessonRepo;
            _topicRepo = topicRepo;
            _subtopicRepo = subtopicRepo;
            _answerRepo = answerRepo;
            _quizRepo = quizRepo;
            _enrollmentService = enrollmentService;
            _subscriptionService = subscriptionService;
        }

        private string CurrentUserId => User.FindFirst("uid")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Unauthorized");

        // 1) GET /api/student/courses?category=linux|pentest
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] string? category = null)
        {
            var courses = await _courseRepo.GetAllCourseAsync().ToListAsync();
            // Support multi-category filter: category=linux|pentest
            HashSet<string>? categories = null;
            if (!string.IsNullOrWhiteSpace(category))
            {
                categories = new HashSet<string>(
                    category.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                    StringComparer.OrdinalIgnoreCase
                );
            }

            var data = courses
                .Select(c => new
                {
                    uid = c.Uid,
                    slug = c.Slug ?? string.Empty,
                    title = c.Title,
                    description = c.Description,
                    coverImageUrl = (string?)null
                })
                .Where(x => categories == null || categories.Contains(x.slug ?? string.Empty))
                .ToList();
            return Ok(new { data });
        }

    // 2) GET /api/student/courses/slug/{slug}/outline
    [HttpGet("courses/slug/{slug}/outline")]
        public async Task<IActionResult> GetCourseOutlineBySlug(string slug)
        {
            var course = await _courseRepo.GetAllCourseAsync()
                .FirstOrDefaultAsync(c => c.Slug == slug.ToLower());
            if (course == null) return NotFound(new { error = new { code = 404, message = "Course not found" } });

            var modules = await _moduleRepo.GetAllAsync()
                .Where(m => m.CourseUid == course.Uid)
                .OrderBy(m => m.OrderIndex)
                .ToListAsync();

            var lessonsAll = await _lessonRepo.GetAllAsync().ToListAsync();
            var topicsAll = await _topicRepo.GetAllAsync().ToListAsync();
            var subtopicsAll = await _subtopicRepo.GetAllAsync().ToListAsync();

            var moduleDtos = modules.Select(m => new
            {
                uid = m.Uid,
                title = m.Title,
                orderIndex = m.OrderIndex ?? 0,
                lessons = lessonsAll
                    .Where(l => l.ModuleUid == m.Uid)
                    .OrderBy(l => l.OrderIndex)
                    .Select(l => new
                    {
                        uid = l.Uid,
                        title = l.Title,
                        orderIndex = l.OrderIndex ?? 0,
                        topics = topicsAll
                            .Where(t => t.LessonUid == l.Uid)
                            .OrderBy(t => t.OrderIndex)
                            .Select(t => new
                            {
                                uid = t.Uid,
                                title = t.Title,
                                orderIndex = t.OrderIndex ?? 0,
                                subtopics = subtopicsAll
                                    .Where(s => s.TopicUid == t.Uid)
                                    .OrderBy(s => s.OrderIndex)
                                    .Select(s => new
                                    {
                                        uid = s.Uid,
                                        title = s.Title,
                                        orderIndex = s.OrderIndex ?? 0
                                    })
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            }).ToList();

            var data = new
            {
                course = new { uid = course.Uid, slug = course.Slug ?? string.Empty, title = course.Title, description = course.Description },
                modules = moduleDtos
            };

            return Ok(new { data });
        }

    // 2b) GET /api/student/courses/{courseUid}/outline (by UID)
    [HttpGet("courses/{courseUid}/outline"), ActionName("GetCourseOutlineByUid")]
        public async Task<IActionResult> GetCourseOutlineByUid(string courseUid)
        {
            var course = await _courseRepo.GetAllCourseAsync()
                .FirstOrDefaultAsync(c => c.Uid == courseUid);
            if (course == null) return NotFound(new { error = new { code = 404, message = "Course not found" } });

            var modules = await _moduleRepo.GetAllAsync()
                .Where(m => m.CourseUid == course.Uid)
                .OrderBy(m => m.OrderIndex)
                .ToListAsync();

            var lessonsAll = await _lessonRepo.GetAllAsync().ToListAsync();
            var topicsAll = await _topicRepo.GetAllAsync().ToListAsync();
            var subtopicsAll = await _subtopicRepo.GetAllAsync().ToListAsync();

            var moduleDtos = modules.Select(m => new
            {
                uid = m.Uid,
                title = m.Title,
                orderIndex = m.OrderIndex ?? 0,
                lessons = lessonsAll
                    .Where(l => l.ModuleUid == m.Uid)
                    .OrderBy(l => l.OrderIndex)
                    .Select(l => new
                    {
                        uid = l.Uid,
                        title = l.Title,
                        orderIndex = l.OrderIndex ?? 0,
                        topics = topicsAll
                            .Where(t => t.LessonUid == l.Uid)
                            .OrderBy(t => t.OrderIndex)
                            .Select(t => new
                            {
                                uid = t.Uid,
                                title = t.Title,
                                orderIndex = t.OrderIndex ?? 0,
                                subtopics = subtopicsAll
                                    .Where(s => s.TopicUid == t.Uid)
                                    .OrderBy(s => s.OrderIndex)
                                    .Select(s => new
                                    {
                                        uid = s.Uid,
                                        title = s.Title,
                                        orderIndex = s.OrderIndex ?? 0
                                    })
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            }).ToList();

            var data = new
            {
                course = new { uid = course.Uid, slug = course.Slug ?? string.Empty, title = course.Title, description = course.Description },
                modules = moduleDtos
            };

            return Ok(new { data });
        }

        // 3) GET /api/lessons/{lessonUid}
        [HttpGet("lessons/{lessonUid}")]
        public async Task<IActionResult> GetLesson(string lessonUid)
        {
            var lesson = await _lessonRepo.GetByIdAsync(lessonUid);
            if (lesson == null) return NotFound(new { error = new { code = 404, message = "Lesson not found" } });

            var topics = await _topicRepo.GetAllAsync()
                .Where(t => t.LessonUid == lessonUid)
                .OrderBy(t => t.OrderIndex)
                .Select(t => new { uid = t.Uid, title = t.Title, orderIndex = t.OrderIndex ?? 0 })
                .ToListAsync();

            var data = new
            {
                uid = lesson.Uid,
                title = lesson.Title,
                orderIndex = lesson.OrderIndex ?? 0,
                topics
            };
            return Ok(new { data });
        }

        // 4) GET /api/topics/{topicUid}
        [HttpGet("topics/{topicUid}")]
        public async Task<IActionResult> GetTopic(string topicUid)
        {
            var topic = await _topicRepo.GetByIdAsync(topicUid);
            if (topic == null) return NotFound(new { error = new { code = 404, message = "Topic not found" } });

            var subtopics = await _subtopicRepo.GetAllAsync()
                .Where(s => s.TopicUid == topicUid)
                .OrderBy(s => s.OrderIndex)
                .Select(s => new { uid = s.Uid, title = s.Title, orderIndex = s.OrderIndex ?? 0 })
                .ToListAsync();

            var data = new
            {
                uid = topic.Uid,
                title = topic.Title,
                orderIndex = topic.OrderIndex ?? 0,
                subtopics
            };
            return Ok(new { data });
        }

        // 5) GET /api/subtopics/{subtopicUid}
        [HttpGet("subtopics/{subtopicUid}")]
        public async Task<IActionResult> GetSubtopic(string subtopicUid)
        {
            var st = await _subtopicRepo.GetByIdAsync(subtopicUid);
            if (st == null) return NotFound(new { error = new { code = 404, message = "Subtopic not found" } });
            var data = new
            {
                uid = st.Uid,
                title = st.Title,
                orderIndex = st.OrderIndex ?? 0,
                contentHtml = st.Content
            };
            return Ok(new { data });
        }

        // 6) POST /api/subtopics/{subtopicUid}/progress { progress: number }
        [HttpPost("subtopics/{subtopicUid}/progress")]
        public async Task<IActionResult> UpdateSubtopicProgress(string subtopicUid, [FromBody] UpdateProgressRequest body)
        {
            if (body == null) return BadRequest(new { error = new { code = 422, message = "Invalid payload" } });

            var studentId = CurrentUserId;
            var existing = await _answerRepo.GetUserProgressAsync(studentId, subtopicUid);
            var now = DateTime.UtcNow;
            if (existing == null)
            {
                existing = new SubtopicProgress
                {
                    Uid = Guid.NewGuid().ToString(),
                    StudentUid = studentId,
                    SubtopicUid = subtopicUid,
                    AttemptCount = 0,
                };
            }
            if (body.Progress >= 100)
            {
                existing.IsCompleted = true;
                existing.CompletedAt = now;
            }
            existing.LastAttemptedAt = now;

            await _answerRepo.CreateOrUpdateProgressAsync(existing);

            var data = new
            {
                subtopicUid,
                progress = body.Progress,
                updatedAt = now
            };
            return Ok(new { data });
        }

        // 7) GET /api/quizzes
        [HttpGet("quizzes")]
        public async Task<IActionResult> ListQuizzes([FromQuery] string? courseSlug = null, [FromQuery] string? moduleUid = null, [FromQuery] string? lessonUid = null)
        {
            // Build from lessons -> quizzes to avoid relying on repo internals
            var lessonsQuery = _lessonRepo.GetAllAsync();
            if (!string.IsNullOrEmpty(lessonUid))
            {
                lessonsQuery = lessonsQuery.Where(l => l.Uid == lessonUid);
            }

            var lessons = await lessonsQuery.ToListAsync();

            if (!string.IsNullOrEmpty(moduleUid) || !string.IsNullOrEmpty(courseSlug))
            {
                var modules = await _moduleRepo.GetAllAsync().ToListAsync();
                var courses = await _courseRepo.GetAllCourseAsync().ToListAsync();

                if (!string.IsNullOrEmpty(moduleUid))
                {
                    lessons = lessons.Where(l => l.ModuleUid == moduleUid).ToList();
                }

                if (!string.IsNullOrEmpty(courseSlug))
                {
                    var course = courses.FirstOrDefault(c => c.Slug == courseSlug.ToLower());
                    if (course != null)
                    {
                        var moduleIds = modules.Where(m => m.CourseUid == course.Uid).Select(m => m.Uid).ToHashSet();
                        lessons = lessons.Where(l => moduleIds.Contains(l.ModuleUid)).ToList();
                    }
                    else
                    {
                        lessons = new List<Lesson>();
                    }
                }
            }

            var items = new List<object>();
            foreach (var l in lessons)
            {
                // Assuming one quiz per lesson; adjust if multiple are supported
                var q = await _quizRepo.GetQuizByLessonIdAsync(l.Uid);
                if (q == null) continue;
                var questions = await _quizRepo.GetQuizQuestionsAsync(q.Uid);
                items.Add(new
                {
                    uid = q.Uid,
                    title = q.Title,
                    description = q.Description,
                    lessonUid = q.LessonUid,
                    moduleUid = l.ModuleUid,
                    numQuestions = questions.Count,
                    timeLimitSeconds = (int?)null
                });
            }

            return Ok(new { data = items });
        }

        // 8) GET /api/quizzes/{quizUid}?includeCorrect=true
        // For learning environment: includeCorrect defaults to TRUE to enable instant feedback
        // For production: set ?includeCorrect=false to hide correct answers
        [HttpGet("quizzes/{quizUid}")]
        public async Task<IActionResult> GetQuiz(string quizUid, [FromQuery] bool includeCorrect = true)
        {
            var quiz = await _quizRepo.GetByIdAsync(quizUid);
            if (quiz == null) return NotFound(new { error = new { code = 404, message = "Quiz not found" } });

            var questions = await _quizRepo.GetQuizQuestionsAsync(quizUid);
            var questionDtos = new List<object>();
            foreach (var q in questions.OrderBy(x => x.OrderIndex))
            {
                var answers = await _quizRepo.GetQuestionAnswersAsync(q.Uid);
                
                // Build answer DTOs with or without isCorrect based on includeCorrect flag
                var answerDtos = answers.Select(a => new 
                { 
                    uid = a.Uid, 
                    content = a.AnswerText,
                    isCorrect = includeCorrect ? (a.IsCorrect ?? false) : (bool?)null
                }).ToList();

                questionDtos.Add(new
                {
                    uid = q.Uid,
                    content = q.QuestionText,
                    orderIndex = q.OrderIndex ?? 0,
                    multipleChoice = string.Equals(q.QuestionType, "multiple_choice", StringComparison.OrdinalIgnoreCase),
                    answers = answerDtos
                });
            }

            var data = new
            {
                uid = quiz.Uid,
                title = quiz.Title,
                description = quiz.Description,
                timeLimitSeconds = (int?)null,
                questions = questionDtos
            };
            return Ok(new { data });
        }

        // 9) POST /api/quiz-submissions
        [HttpPost("quiz-submissions")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.QuizUid))
                return BadRequest(new { error = new { code = 422, message = "quizUid is required" } });

            var quiz = await _quizRepo.GetByIdAsync(req.QuizUid);
            if (quiz == null) return NotFound(new { error = new { code = 404, message = "Quiz not found" } });

            var studentId = CurrentUserId;

            // Ensure not previously submitted
            var existing = await _quizRepo.GetUserSubmissionAsync(req.QuizUid, studentId);
            if (existing != null)
            {
                return BadRequest(new { error = new { code = 422, message = "You have already submitted this quiz" } });
            }

            var questions = await _quizRepo.GetQuizQuestionsAsync(req.QuizUid);
            var questionMap = questions.ToDictionary(q => q.Uid);
            int totalQuestions = questions.Count;
            int correctCount = 0;
            var breakdown = new List<object>();

            foreach (var q in questions)
            {
                var selected = req.Answers?.FirstOrDefault(a => a.QuestionUid == q.Uid)?.SelectedAnswerUids ?? new List<string>();
                var allAnswers = await _quizRepo.GetQuestionAnswersAsync(q.Uid);
                var correctAnswers = allAnswers.Where(a => a.IsCorrect == true).Select(a => a.Uid).OrderBy(x => x).ToList();
                var selectedSorted = selected.Distinct().OrderBy(x => x).ToList();
                bool isCorrect = correctAnswers.SequenceEqual(selectedSorted);
                if (isCorrect) correctCount++;
                breakdown.Add(new { questionUid = q.Uid, isCorrect, correctAnswerUids = correctAnswers });
            }

            decimal score = totalQuestions > 0 ? ((decimal)correctCount / totalQuestions) * 100 : 0;

            // Save submission
            var submission = new QuizSubmission
            {
                Uid = Guid.NewGuid().ToString(),
                QuizUid = req.QuizUid,
                StudentUid = studentId,
                Score = score,
                SubmittedAt = DateTime.UtcNow
            };
            await _quizRepo.CreateSubmissionAsync(submission);

            // Save answers (one row per selected answer)
            var submissionAnswers = new List<QuizSubmissionAnswer>();
            foreach (var ans in req.Answers ?? new List<QuizSubmissionAnswerRequest>())
            {
                if (ans.SelectedAnswerUids == null) continue;
                foreach (var sel in ans.SelectedAnswerUids.Distinct())
                {
                    // verify the answer belongs to the question to avoid bad data
                    submissionAnswers.Add(new QuizSubmissionAnswer
                    {
                        Uid = Guid.NewGuid().ToString(),
                        SubmissionUid = submission.Uid,
                        QuestionUid = ans.QuestionUid,
                        SelectedAnswerUid = sel,
                        IsCorrect = null // optional per-answer flag; grading is in breakdown
                    });
                }
            }
            if (submissionAnswers.Count > 0)
            {
                await _quizRepo.CreateSubmissionAnswersAsync(submissionAnswers);
            }

            var data = new
            {
                submissionUid = submission.Uid,
                score,
                correctCount,
                totalQuestions,
                startedAt = (DateTime?)null,
                submittedAt = submission.SubmittedAt,
                breakdown
            };
            return Ok(new { data });
        }

        // 10) GET /api/quiz-submissions/{submissionUid}
        [HttpGet("quiz-submissions/{submissionUid}")]
        public async Task<IActionResult> GetQuizSubmission(string submissionUid)
        {
            var submission = await _quizRepo.GetSubmissionByIdAsync(submissionUid);
            if (submission == null) return NotFound(new { error = new { code = 404, message = "Submission not found" } });
            if (submission.StudentUid != CurrentUserId) return Forbid();

            var quiz = await _quizRepo.GetByIdAsync(submission.QuizUid);
            var questions = await _quizRepo.GetQuizQuestionsAsync(submission.QuizUid);
            questions = questions.OrderBy(q => q.OrderIndex).ToList();
            var submissionAnswers = await _quizRepo.GetSubmissionAnswersAsync(submission.Uid);

            var dataBreakdown = new List<object>();
            int correctCount2 = 0;
            foreach (var q in questions)
            {
                var selected = submissionAnswers.Where(sa => sa.QuestionUid == q.Uid).Select(sa => sa.SelectedAnswerUid).ToList();
                var allAnswers = await _quizRepo.GetQuestionAnswersAsync(q.Uid);
                var correct = allAnswers.Where(a => a.IsCorrect == true).Select(a => a.Uid).ToList();
                var isCorrect = selected.OrderBy(x => x).SequenceEqual(correct.OrderBy(x => x));
                if (isCorrect) correctCount2++;
                dataBreakdown.Add(new
                {
                    questionUid = q.Uid,
                    questionContent = q.QuestionText,
                    isCorrect,
                    selectedAnswerUids = selected,
                    correctAnswerUids = correct
                });
            }

            var data = new
            {
                submissionUid = submission.Uid,
                quizUid = submission.QuizUid,
                quizTitle = quiz?.Title,
                score = submission.Score,
                correctCount = correctCount2,
                totalQuestions = questions.Count,
                startedAt = (DateTime?)null,
                submittedAt = submission.SubmittedAt,
                breakdown = dataBreakdown
            };
            return Ok(new { data });
        }

        // 11) GET /api/users/me/progress/courses/{courseUid}
        [HttpGet("users/me/progress/courses/{courseUid}")]
        public async Task<IActionResult> GetCourseProgressSummary(string courseUid)
        {
            var userId = CurrentUserId;
            var modules = await _moduleRepo.GetAllAsync().Where(m => m.CourseUid == courseUid).ToListAsync();
            var lessons = await _lessonRepo.GetAllAsync().ToListAsync();
            var topics = await _topicRepo.GetAllAsync().ToListAsync();
            var subtopics = await _subtopicRepo.GetAllAsync().ToListAsync();

            var totalSubtopics = subtopics.Count(st => topics.Any(t => t.Uid == st.TopicUid && lessons.Any(l => l.Uid == t.LessonUid && modules.Any(m => m.Uid == l.ModuleUid))));
            int completedSubtopics = 0;
            foreach (var st in subtopics)
            {
                var t = topics.FirstOrDefault(x => x.Uid == st.TopicUid);
                var l = lessons.FirstOrDefault(x => x.Uid == t?.LessonUid);
                if (t == null || l == null) continue;
                if (!modules.Any(m => m.Uid == l.ModuleUid)) continue;
                var prog = await _answerRepo.GetUserProgressAsync(userId, st.Uid);
                if (prog?.IsCompleted == true) completedSubtopics++;
            }

            // Quizzes stats
            var moduleIds = modules.Select(m => m.Uid).ToHashSet();
            var lessonIds = lessons.Where(l => moduleIds.Contains(l.ModuleUid)).Select(l => l.Uid).ToHashSet();
            // Build course quizzes by checking lessons
            var courseQuizzes = new List<Quiz>();
            foreach (var l in lessons.Where(l => lessonIds.Contains(l.Uid)))
            {
                var q = await _quizRepo.GetQuizByLessonIdAsync(l.Uid);
                if (q != null) courseQuizzes.Add(q);
            }
            int completedQuizzes = 0;
            decimal avgScoreSum = 0;
            int submissionCount = 0;
            foreach (var q in courseQuizzes)
            {
                var sub = await _quizRepo.GetUserSubmissionAsync(q.Uid, userId);
                if (sub != null)
                {
                    completedQuizzes++;
                    if (sub.Score.HasValue)
                    {
                        avgScoreSum += sub.Score.Value;
                        submissionCount++;
                    }
                }
            }
            decimal? averageScore = submissionCount > 0 ? avgScoreSum / submissionCount : null;

            var course = await _courseRepo.GetByIdAsync(courseUid);
            var data = new
            {
                courseUid = course?.Uid ?? courseUid,
                courseTitle = course?.Title,
                completedSubtopics,
                totalSubtopics,
                progressPercentage = totalSubtopics > 0 ? (int)Math.Round((decimal)completedSubtopics * 100 / totalSubtopics) : 0,
                quizzes = new { completed = completedQuizzes, total = courseQuizzes.Count, averageScore }
            };
            return Ok(new { data });
        }

        // 12) GET /api/users/me/progress/lessons/{lessonUid}
        [HttpGet("users/me/progress/lessons/{lessonUid}")]
        public async Task<IActionResult> GetLessonProgressSummary(string lessonUid)
        {
            var userId = CurrentUserId;
            var topics = await _topicRepo.GetAllAsync().Where(t => t.LessonUid == lessonUid).ToListAsync();
            var topicIds = topics.Select(t => t.Uid).ToHashSet();
            var subtopics = await _subtopicRepo.GetAllAsync().Where(st => topicIds.Contains(st.TopicUid)).ToListAsync();
            int totalSubtopics = subtopics.Count;
            int completedSubtopics = 0;
            foreach (var st in subtopics)
            {
                var prog = await _answerRepo.GetUserProgressAsync(userId, st.Uid);
                if (prog?.IsCompleted == true) completedSubtopics++;
            }

            var lesson = await _lessonRepo.GetByIdAsync(lessonUid);
            var data = new
            {
                lessonUid = lesson?.Uid ?? lessonUid,
                lessonTitle = lesson?.Title,
                completedSubtopics,
                totalSubtopics,
                progressPercentage = totalSubtopics > 0 ? (int)Math.Round((decimal)completedSubtopics * 100 / totalSubtopics) : 0
            };
            return Ok(new { data });
        }

        // 13) POST /api/courses/{courseUid}/enroll
        [HttpPost("courses/{courseUid}/enroll")]
        public async Task<IActionResult> EnrollCourse(string courseUid)
        {
            var result = await _enrollmentService.EnrollAsync(courseUid, CurrentUserId);
            if (!result) return BadRequest(new { error = new { code = 422, message = "Already enrolled or failed" } });
            return Ok(new { data = new { enrollmentUid = (string?)null, courseUid, userId = CurrentUserId, enrolledAt = DateTime.UtcNow } });
        }

        // 14) GET /api/users/me/enrollments
        [HttpGet("users/me/enrollments")]
        public async Task<IActionResult> GetMyEnrollments([FromQuery] string? category = null)
        {
            var userId = CurrentUserId;
            var enrollments = await _enrollmentService.GetMyEnrollmentsAsync(userId);
            var courses = await _courseRepo.GetAllCourseAsync().ToListAsync();
            // Support multi-category filter: category=linux|pentest
            HashSet<string>? categories = null;
            if (!string.IsNullOrWhiteSpace(category))
            {
                categories = new HashSet<string>(
                    category.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                    StringComparer.OrdinalIgnoreCase
                );
            }

            var data = enrollments.Select(e =>
            {
                var c = courses.FirstOrDefault(x => x.Uid == e.CourseUid);
                return new
                {
                    enrollmentUid = (string?)null,
                    courseUid = e.CourseUid,
                    courseSlug = c?.Slug ?? string.Empty,
                    courseTitle = c?.Title,
                    enrolledAt = e.EnrolledAt
                };
            }).Where(x => categories == null || categories.Contains(x.courseSlug ?? string.Empty)).ToList();

            return Ok(new { data });
        }

        // 15) GET /api/student/subscription/access
        [HttpGet("subscription/access")]
        public async Task<IActionResult> GetSubscriptionAccess()
        {
            try
            {
                var userId = CurrentUserId;
                var subscriptionAccess = await _subscriptionService.CheckUserSubscriptionAccessAsync(userId);

                var message = subscriptionAccess.HasAccess 
                    ? "User has active subscription" 
                    : "User has no active subscription";

                return Ok(new { success = true, data = subscriptionAccess, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // 16) GET /api/student/courses/{courseUid}/modules/{moduleIndex}/access
        [HttpGet("courses/{courseUid}/modules/{moduleIndex}/access")]
        public async Task<IActionResult> CheckModuleAccess(string courseUid, int moduleIndex)
        {
            try
            {
                var userId = CurrentUserId;
                var moduleAccess = await _subscriptionService.CheckModuleAccessAsync(userId, courseUid, moduleIndex);

                return Ok(new { success = true, data = moduleAccess });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public class UpdateProgressRequest
        {
            public int Progress { get; set; }
        }

        public class QuizSubmissionRequest
        {
            public string QuizUid { get; set; }
            public List<QuizSubmissionAnswerRequest> Answers { get; set; } = new();
        }

        public class QuizSubmissionAnswerRequest
        {
            public string QuestionUid { get; set; }
            public List<string> SelectedAnswerUids { get; set; } = new();
        }
    }
}
