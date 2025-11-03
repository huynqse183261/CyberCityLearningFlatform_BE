using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Teacher;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public interface ITeacherRepository
    {
        Task<(List<TeacherStudentListDto> students, int totalCount)> GetStudentsAsync(string teacherUid, int page, int limit, string? search, string? courseUid);
        Task<TeacherStudentListDto?> GetStudentDetailAsync(string teacherUid, string studentUid);
        Task<AddStudentDataDto> AddStudentAsync(string teacherUid, string studentUid, string courseUid);
        Task<bool> RemoveStudentAsync(string teacherUid, string studentUid, string courseUid);
        Task<List<TeacherCourseProgressDto>> GetStudentProgressAsync(string teacherUid, string studentUid, string? courseUid);
        Task<(List<TeacherConversationDto> conversations, int totalCount)> GetConversationsAsync(string teacherUid, int page, int limit);
        Task<string?> CreateConversationAsync(string teacherUid, string studentUid);
        Task<string?> FindExistingConversationAsync(string teacherUid, string studentUid);
        Task<(List<TeacherMessageDto> messages, int totalCount, ParticipantDto? participant)> GetConversationMessagesAsync(string teacherUid, string conversationUid, int page, int limit, DateTime? before);
        Task<TeacherMessageDto?> SendMessageAsync(string teacherUid, string conversationUid, string content);
        Task<bool> MarkAsReadAsync(string teacherUid, string conversationUid);
        Task<DashboardStatsDataDto> GetDashboardStatsAsync(string teacherUid);
        Task<bool> VerifyTeacherStudentRelationship(string teacherUid, string studentUid);
    }

    public class TeacherRepository : GenericRepository<TeacherStudent>, ITeacherRepository
    {
        public TeacherRepository() { }
        public TeacherRepository(CyberCityLearningFlatFormDBContext context) => _context = context;

        public async Task<(List<TeacherStudentListDto> students, int totalCount)> GetStudentsAsync(
            string teacherUid, int page, int limit, string? search, string? courseUid)
        {
            var query = _context.TeacherStudents
                .Where(ts => ts.TeacherUid == teacherUid)
                .Select(ts => ts.StudentUid)
                .Distinct();

            var studentsQuery = _context.Users
                .Where(u => query.Contains(u.Uid));

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                studentsQuery = studentsQuery.Where(u =>
                    EF.Functions.ILike(u.FullName ?? "", $"%{search}%") ||
                    EF.Functions.ILike(u.Email ?? "", $"%{search}%"));
            }

            var totalCount = await studentsQuery.CountAsync();

            var students = await studentsQuery
                .OrderBy(u => u.FullName)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(u => new
                {
                    u.Uid,
                    u.FullName,
                    u.Email,
                    u.Username,
                    u.CreatedAt
                })
                .ToListAsync();

            var result = new List<TeacherStudentListDto>();

            foreach (var student in students)
            {
                var coursesQuery = _context.CourseEnrollments
                    .Where(ce => ce.UserUid == student.Uid)
                    .Join(_context.Courses,
                        ce => ce.CourseUid,
                        c => c.Uid,
                        (ce, c) => new { ce, c });

                if (courseUid != null)
                {
                    coursesQuery = coursesQuery.Where(x => x.c.Uid == courseUid);
                }

                var courses = await coursesQuery
                    .Select(x => new EnrolledCourseDto
                    {
                        CourseUid = x.c.Uid,
                        CourseTitle = x.c.Title ?? string.Empty,
                        EnrolledAt = x.ce.EnrolledAt
                    })
                    .ToListAsync();

                if (courseUid == null || courses.Any())
                {
                    result.Add(new TeacherStudentListDto
                    {
                        Uid = student.Uid,
                        FullName = student.FullName ?? string.Empty,
                        Email = student.Email ?? string.Empty,
                        Username = student.Username ?? string.Empty,
                        EnrolledCourses = courses,
                        LastActive = student.CreatedAt
                    });
                }
            }

            return (result, totalCount);
        }

        public async Task<TeacherStudentListDto?> GetStudentDetailAsync(string teacherUid, string studentUid)
        {
            var exists = await _context.TeacherStudents
                .AnyAsync(ts => ts.TeacherUid == teacherUid && ts.StudentUid == studentUid);

            if (!exists) return null;

            var student = await _context.Users
                .Where(u => u.Uid == studentUid)
                .Select(u => new
                {
                    u.Uid,
                    u.FullName,
                    u.Email,
                    u.Username,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (student == null) return null;

            var courses = await _context.CourseEnrollments
                .Where(ce => ce.UserUid == studentUid)
                .Join(_context.Courses,
                    ce => ce.CourseUid,
                    c => c.Uid,
                    (ce, c) => new EnrolledCourseDto
                    {
                        CourseUid = c.Uid,
                        CourseTitle = c.Title ?? string.Empty,
                        EnrolledAt = ce.EnrolledAt
                    })
                .ToListAsync();

            return new TeacherStudentListDto
            {
                Uid = student.Uid,
                FullName = student.FullName ?? string.Empty,
                Email = student.Email ?? string.Empty,
                Username = student.Username ?? string.Empty,
                EnrolledCourses = courses,
                LastActive = student.CreatedAt
            };
        }

        public async Task<AddStudentDataDto> AddStudentAsync(string teacherUid, string studentUid, string courseUid)
        {
            // Check if relationship already exists
            var existing = await _context.TeacherStudents
                .FirstOrDefaultAsync(ts => ts.TeacherUid == teacherUid &&
                                          ts.StudentUid == studentUid &&
                                          ts.CourseUid == courseUid);

            if (existing != null)
            {
                return new AddStudentDataDto
                {
                    Uid = existing.Uid,
                    TeacherUid = existing.TeacherUid,
                    StudentUid = existing.StudentUid,
                    CourseUid = existing.CourseUid,
                    AssignedAt = DateTime.Now
                };
            }

            // Add to teacher_student
            var teacherStudent = new TeacherStudent
            {
                Uid = Guid.NewGuid().ToString(),
                TeacherUid = teacherUid,
                StudentUid = studentUid,
                CourseUid = courseUid
            };

            await _context.TeacherStudents.AddAsync(teacherStudent);

            // Enroll student if not already
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(ce => ce.UserUid == studentUid && ce.CourseUid == courseUid);

            if (enrollment == null)
            {
                await _context.CourseEnrollments.AddAsync(new CourseEnrollment
                {
                    Uid = Guid.NewGuid().ToString(),
                    UserUid = studentUid,
                    CourseUid = courseUid,
                    EnrolledAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return new AddStudentDataDto
            {
                Uid = teacherStudent.Uid,
                TeacherUid = teacherStudent.TeacherUid,
                StudentUid = teacherStudent.StudentUid,
                CourseUid = teacherStudent.CourseUid,
                AssignedAt = DateTime.Now
            };
        }

        public async Task<bool> RemoveStudentAsync(string teacherUid, string studentUid, string courseUid)
        {
            var record = await _context.TeacherStudents
                .FirstOrDefaultAsync(ts => ts.TeacherUid == teacherUid &&
                                          ts.StudentUid == studentUid &&
                                          ts.CourseUid == courseUid);

            if (record == null) return false;

            _context.TeacherStudents.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TeacherCourseProgressDto>> GetStudentProgressAsync(
            string teacherUid, string studentUid, string? courseUid)
        {
            // Verify relationship
            var hasRelationship = await _context.TeacherStudents
                .AnyAsync(ts => ts.TeacherUid == teacherUid && ts.StudentUid == studentUid);

            if (!hasRelationship) return new List<TeacherCourseProgressDto>();

            var coursesQuery = _context.Courses.AsQueryable();

            if (courseUid != null)
            {
                coursesQuery = coursesQuery.Where(c => c.Uid == courseUid);
            }
            else
            {
                // Get courses where student is enrolled
                var enrolledCourseIds = _context.CourseEnrollments
                    .Where(ce => ce.UserUid == studentUid)
                    .Select(ce => ce.CourseUid);
                coursesQuery = coursesQuery.Where(c => enrolledCourseIds.Contains(c.Uid));
            }

            var courses = await coursesQuery.ToListAsync();
            var result = new List<TeacherCourseProgressDto>();

            foreach (var course in courses)
            {
                var modules = await _context.Modules
                    .Where(m => m.CourseUid == course.Uid)
                    .OrderBy(m => m.OrderIndex)
                    .ToListAsync();

                var courseProgress = new TeacherCourseProgressDto
                {
                    CourseUid = course.Uid,
                    CourseTitle = course.Title ?? string.Empty,
                    Modules = new List<TeacherModuleProgressDto>()
                };

                int courseTotalSubtopics = 0;
                int courseCompletedSubtopics = 0;

                foreach (var module in modules)
                {
                    var lessons = await _context.Lessons
                        .Where(l => l.ModuleUid == module.Uid)
                        .OrderBy(l => l.OrderIndex)
                        .ToListAsync();

                    var moduleProgress = new TeacherModuleProgressDto
                    {
                        ModuleUid = module.Uid,
                        ModuleTitle = module.Title ?? string.Empty,
                        Lessons = new List<TeacherLessonProgressDto>()
                    };

                    int moduleTotalSubtopics = 0;
                    int moduleCompletedSubtopics = 0;

                    foreach (var lesson in lessons)
                    {
                        var subtopicIds = await _context.Topics
                            .Where(t => t.LessonUid == lesson.Uid)
                            .SelectMany(t => _context.Subtopics.Where(s => s.TopicUid == t.Uid))
                            .Select(s => s.Uid)
                            .ToListAsync();

                        var completedCount = await _context.SubtopicProgresses
                            .Where(sp => sp.StudentUid == studentUid &&
                                        subtopicIds.Contains(sp.SubtopicUid) &&
                                        sp.IsCompleted == true)
                            .CountAsync();

                        var totalCount = subtopicIds.Count;
                        moduleTotalSubtopics += totalCount;
                        moduleCompletedSubtopics += completedCount;

                        moduleProgress.Lessons.Add(new TeacherLessonProgressDto
                        {
                            LessonUid = lesson.Uid,
                            LessonTitle = lesson.Title ?? string.Empty,
                            TotalSubtopics = totalCount,
                            CompletedSubtopics = completedCount,
                            Progress = totalCount > 0 ? Math.Round((double)completedCount / totalCount * 100, 2) : 0
                        });
                    }

                    moduleProgress.TotalSubtopics = moduleTotalSubtopics;
                    moduleProgress.CompletedSubtopics = moduleCompletedSubtopics;
                    moduleProgress.Progress = moduleTotalSubtopics > 0 
                        ? Math.Round((double)moduleCompletedSubtopics / moduleTotalSubtopics * 100, 2) 
                        : 0;

                    courseTotalSubtopics += moduleTotalSubtopics;
                    courseCompletedSubtopics += moduleCompletedSubtopics;

                    courseProgress.Modules.Add(moduleProgress);
                }

                courseProgress.TotalSubtopics = courseTotalSubtopics;
                courseProgress.CompletedSubtopics = courseCompletedSubtopics;
                courseProgress.OverallProgress = courseTotalSubtopics > 0 
                    ? Math.Round((double)courseCompletedSubtopics / courseTotalSubtopics * 100, 2) 
                    : 0;

                courseProgress.LastActivity = await _context.SubtopicProgresses
                    .Where(sp => sp.StudentUid == studentUid)
                    .MaxAsync(sp => (DateTime?)sp.CompletedAt);

                result.Add(courseProgress);
            }

            return result;
        }

        public async Task<(List<TeacherConversationDto> conversations, int totalCount)> GetConversationsAsync(
            string teacherUid, int page, int limit)
        {
            // Get conversations where teacher is a member
            var conversationsQuery = _context.ConversationMembers
                .Where(cm => cm.UserUid == teacherUid)
                .Select(cm => cm.ConversationUid);

            var totalCount = await conversationsQuery.Distinct().CountAsync();

            var conversationUids = await conversationsQuery
                .Distinct()
                .OrderByDescending(uid => _context.Messages
                    .Where(m => m.ConversationUid == uid)
                    .Max(m => (DateTime?)m.SentAt) ?? DateTime.MinValue)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var result = new List<TeacherConversationDto>();

            foreach (var convUid in conversationUids)
            {
                // Get the other participant (not the teacher)
                var otherMemberUid = await _context.ConversationMembers
                    .Where(cm => cm.ConversationUid == convUid && cm.UserUid != teacherUid)
                    .Select(cm => cm.UserUid)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(otherMemberUid)) continue;

                var participant = await _context.Users
                    .Where(u => u.Uid == otherMemberUid)
                    .Select(u => u.FullName)
                    .FirstOrDefaultAsync();

                var lastMessage = await _context.Messages
                    .Where(m => m.ConversationUid == convUid)
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new { m.Message1, m.SentAt })
                    .FirstOrDefaultAsync();

                var unreadCount = await _context.Messages
                    .Where(m => m.ConversationUid == convUid && m.SenderUid != teacherUid)
                    .CountAsync();

                result.Add(new TeacherConversationDto
                {
                    ConversationUid = convUid,
                    ParticipantUid = otherMemberUid,
                    ParticipantName = participant ?? string.Empty,
                    LastMessage = lastMessage?.Message1,
                    LastMessageAt = lastMessage?.SentAt,
                    UnreadCount = unreadCount
                });
            }

            return (result, totalCount);
        }

        public async Task<string?> FindExistingConversationAsync(string teacherUid, string studentUid)
        {
            // Find conversation where both teacher and student are members
            var teacherConversations = _context.ConversationMembers
                .Where(cm => cm.UserUid == teacherUid)
                .Select(cm => cm.ConversationUid);

            var studentConversations = _context.ConversationMembers
                .Where(cm => cm.UserUid == studentUid)
                .Select(cm => cm.ConversationUid);

            var sharedConversation = await teacherConversations
                .Intersect(studentConversations)
                .FirstOrDefaultAsync();

            return string.IsNullOrEmpty(sharedConversation) ? null : sharedConversation;
        }

        public async Task<string?> CreateConversationAsync(string teacherUid, string studentUid)
        {
            var conversation = new Conversation
            {
                Uid = Guid.NewGuid().ToString(),
                IsGroup = false,
                CreatedAt = DateTime.Now
            };

            await _context.Conversations.AddAsync(conversation);

            // Add both members
            
            await _context.ConversationMembers.AddAsync(new ConversationMember
            {
                Uid = Guid.NewGuid().ToString(),
                ConversationUid = conversation.Uid,
                UserUid = teacherUid,
                JoinedAt = DateTime.Now
            });

            await _context.ConversationMembers.AddAsync(new ConversationMember
            {
                Uid = Guid.NewGuid().ToString(),
                ConversationUid = conversation.Uid,
                UserUid = studentUid,
                JoinedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return conversation.Uid;
        }

        public async Task<(List<TeacherMessageDto> messages, int totalCount, ParticipantDto? participant)> 
            GetConversationMessagesAsync(string teacherUid, string conversationUid, int page, int limit, DateTime? before)
        {
            // Check if teacher is member of this conversation
            var isMember = await _context.ConversationMembers
                .AnyAsync(cm => cm.ConversationUid == conversationUid && cm.UserUid == teacherUid);

            if (!isMember) 
                return (new List<TeacherMessageDto>(), 0, null);

            // Get the other participant
            var participantUid = await _context.ConversationMembers
                .Where(cm => cm.ConversationUid == conversationUid && cm.UserUid != teacherUid)
                .Select(cm => cm.UserUid)
                .FirstOrDefaultAsync();

            ParticipantDto? participant = null;
            if (!string.IsNullOrEmpty(participantUid))
            {
                participant = await _context.Users
                    .Where(u => u.Uid == participantUid)
                    .Select(u => new ParticipantDto
                    {
                        Uid = u.Uid,
                        FullName = u.FullName ?? string.Empty
                    })
                    .FirstOrDefaultAsync();
            }

            var messagesQuery = _context.Messages
                .Where(m => m.ConversationUid == conversationUid);

            if (before.HasValue)
            {
                messagesQuery = messagesQuery.Where(m => m.SentAt < before.Value);
            }

            var totalCount = await messagesQuery.CountAsync();

            var messages = await messagesQuery
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(m => new TeacherMessageDto
                {
                    Uid = m.Uid,
                    SenderUid = m.SenderUid,
                    Content = m.Message1 ?? string.Empty,
                    CreatedAt = m.SentAt ?? DateTime.Now,
                    IsRead = false // Message model doesn't have IsRead field
                })
                .ToListAsync();

            return (messages, totalCount, participant);
        }

        public async Task<TeacherMessageDto?> SendMessageAsync(string teacherUid, string conversationUid, string content)
        {
            // Check if teacher is member of this conversation
            var isMember = await _context.ConversationMembers
                .AnyAsync(cm => cm.ConversationUid == conversationUid && cm.UserUid == teacherUid);

            if (!isMember) return null;

            var message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ConversationUid = conversationUid,
                SenderUid = teacherUid,
                Message1 = content,
                SentAt = DateTime.Now
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return new TeacherMessageDto
            {
                Uid = message.Uid,
                SenderUid = message.SenderUid,
                Content = message.Message1 ?? string.Empty,
                CreatedAt = message.SentAt ?? DateTime.Now,
                IsRead = false
            };
        }

        public async Task<bool> MarkAsReadAsync(string teacherUid, string conversationUid)
        {
            // Check if teacher is member of this conversation
            var isMember = await _context.ConversationMembers
                .AnyAsync(cm => cm.ConversationUid == conversationUid && cm.UserUid == teacherUid);

            if (!isMember) return false;

            // Note: Message model doesn't have IsRead field
            // This is just a placeholder method that returns true
            return true;
        }

        public async Task<DashboardStatsDataDto> GetDashboardStatsAsync(string teacherUid)
        {
            var totalStudents = await _context.TeacherStudents
                .Where(ts => ts.TeacherUid == teacherUid)
                .Select(ts => ts.StudentUid)
                .Distinct()
                .CountAsync();

            var activeCourses = await _context.Courses
                .Where(c => c.CreatedBy == teacherUid)
                .CountAsync();

            // Note: Message model doesn't have IsRead field
            var unreadMessages = 0;

            // Calculate average progress
            var studentIds = await _context.TeacherStudents
                .Where(ts => ts.TeacherUid == teacherUid)
                .Select(ts => ts.StudentUid)
                .Distinct()
                .ToListAsync();

            double avgProgress = 0;
            if (studentIds.Any())
            {
                var progressList = new List<double>();

                foreach (var studentId in studentIds)
                {
                    var totalSubtopics = await _context.Subtopics.CountAsync();
                    if (totalSubtopics > 0)
                    {
                        var completedSubtopics = await _context.SubtopicProgresses
                            .Where(sp => sp.StudentUid == studentId && sp.IsCompleted == true)
                            .CountAsync();

                        progressList.Add((double)completedSubtopics / totalSubtopics * 100);
                    }
                }

                avgProgress = progressList.Any() ? Math.Round(progressList.Average(), 2) : 0;
            }

            return new DashboardStatsDataDto
            {
                TotalStudents = totalStudents,
                ActiveCourses = activeCourses,
                UnreadMessages = unreadMessages,
                AvgProgress = avgProgress
            };
        }

        public async Task<bool> VerifyTeacherStudentRelationship(string teacherUid, string studentUid)
        {
            return await _context.TeacherStudents
                .AnyAsync(ts => ts.TeacherUid == teacherUid && ts.StudentUid == studentUid);
        }
    }
}
