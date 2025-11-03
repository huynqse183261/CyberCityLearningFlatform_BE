using CyberCity.Application.Interface;
using CyberCity.DTOs.Teacher;
using CyberCity.Infrastructure;

namespace CyberCity.Application.Implement
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepo;

        public TeacherService(ITeacherRepository teacherRepo)
        {
            _teacherRepo = teacherRepo;
        }

        public async Task<TeacherStudentListResponse> GetStudentsAsync(string teacherUid, int page, int limit, string? search, string? courseUid)
        {
            try
            {
                var (students, totalCount) = await _teacherRepo.GetStudentsAsync(
                    teacherUid,
                    page,
                    limit,
                    search,
                    courseUid
                );

                return new TeacherStudentListResponse
                {
                    Success = true,
                    Students = students,
                    Pagination = new DTOs.Teacher.PaginationDto
                    {
                        CurrentPage = page,
                        ItemsPerPage = limit,
                        TotalItems = totalCount,
                        TotalPages = (int)Math.Ceiling((double)totalCount / limit)
                    }
                };
            }
            catch (Exception ex)
            {
                return new TeacherStudentListResponse
                {
                    Success = false,
                    Students = new List<TeacherStudentListDto>(),
                    Pagination = new DTOs.Teacher.PaginationDto()
                };
            }
        }

        public async Task<TeacherStudentDetailResponse> GetStudentDetailAsync(string teacherUid, string studentUid)
        {
            try
            {
                var student = await _teacherRepo.GetStudentDetailAsync(teacherUid, studentUid);

                if (student == null)
                {
                    return new TeacherStudentDetailResponse
                    {
                        Success = false,
                        Data = null
                    };
                }

                return new TeacherStudentDetailResponse
                {
                    Success = true,
                    Data = student
                };
            }
            catch (Exception ex)
            {
                return new TeacherStudentDetailResponse
                {
                    Success = false,
                    Data = null
                };
            }
        }

        public async Task<AddStudentResponse> AddStudentAsync(string teacherUid, AddStudentRequest request)
        {
            try
            {
                var result = await _teacherRepo.AddStudentAsync(
                    teacherUid,
                    request.StudentUid,
                    request.CourseUid
                );

                return new AddStudentResponse
                {
                    Success = true,
                    Message = "Thêm học viên thành công",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new AddStudentResponse
                {
                    Success = false,
                    Message = $"Lỗi khi thêm học viên: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<RemoveStudentResponse> RemoveStudentAsync(string teacherUid, string studentUid, string courseUid)
        {
            try
            {
                var success = await _teacherRepo.RemoveStudentAsync(teacherUid, studentUid, courseUid);

                if (!success)
                {
                    return new RemoveStudentResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy quan hệ giáo viên-học viên"
                    };
                }

                return new RemoveStudentResponse
                {
                    Success = true,
                    Message = "Xóa học viên thành công"
                };
            }
            catch (Exception ex)
            {
                return new RemoveStudentResponse
                {
                    Success = false,
                    Message = $"Lỗi khi xóa học viên: {ex.Message}"
                };
            }
        }

        public async Task<StudentProgressResponse> GetStudentProgressAsync(string teacherUid, string studentUid, string? courseUid)
        {
            try
            {
                var hasRelationship = await _teacherRepo.VerifyTeacherStudentRelationship(teacherUid, studentUid);

                if (!hasRelationship)
                {
                    return new StudentProgressResponse
                    {
                        Success = false,
                        Data = new List<TeacherCourseProgressDto>()
                    };
                }

                var progress = await _teacherRepo.GetStudentProgressAsync(teacherUid, studentUid, courseUid);

                return new StudentProgressResponse
                {
                    Success = true,
                    Data = progress
                };
            }
            catch (Exception ex)
            {
                return new StudentProgressResponse
                {
                    Success = false,
                    Data = new List<TeacherCourseProgressDto>()
                };
            }
        }

        public async Task<ConversationsListResponse> GetConversationsAsync(string teacherUid, int page, int limit)
        {
            try
            {
                var (conversations, totalCount) = await _teacherRepo.GetConversationsAsync(
                    teacherUid,
                    page,
                    limit
                );

                return new ConversationsListResponse
                {
                    Success = true,
                    Conversations = conversations,
                    Pagination = new DTOs.Teacher.PaginationDto
                    {
                        CurrentPage = page,
                        ItemsPerPage = limit,
                        TotalItems = totalCount,
                        TotalPages = (int)Math.Ceiling((double)totalCount / limit)
                    }
                };
            }
            catch (Exception ex)
            {
                return new ConversationsListResponse
                {
                    Success = false,
                    Conversations = new List<TeacherConversationDto>(),
                    Pagination = new DTOs.Teacher.PaginationDto()
                };
            }
        }

        public async Task<CreateConversationResponse> CreateConversationAsync(string teacherUid, CreateConversationRequest request)
        {
            try
            {
                // Check if conversation already exists
                var existingConversationUid = await _teacherRepo.FindExistingConversationAsync(teacherUid, request.StudentUid);

                if (!string.IsNullOrEmpty(existingConversationUid))
                {
                return new CreateConversationResponse
                {
                    Success = true,
                    Message = "Hội thoại đã tồn tại",
                    Data = new CreateConversationDataDto
                    {
                        ConversationUid = existingConversationUid
                    }
                };
                }

                // Create new conversation
                var conversationUid = await _teacherRepo.CreateConversationAsync(teacherUid, request.StudentUid);

                if (string.IsNullOrEmpty(conversationUid))
                {
                    return new CreateConversationResponse
                    {
                        Success = false,
                        Message = "Không thể tạo hội thoại",
                        Data = null
                    };
                }

                return new CreateConversationResponse
                {
                    Success = true,
                    Message = "Tạo hội thoại thành công",
                    Data = new CreateConversationDataDto
                    {
                        ConversationUid = conversationUid
                    }
                };
            }
            catch (Exception ex)
            {
                return new CreateConversationResponse
                {
                    Success = false,
                    Message = $"Lỗi khi tạo hội thoại: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ConversationMessagesResponse> GetConversationMessagesAsync(
            string teacherUid, string conversationUid, int page, int limit, DateTime? before)
        {
            try
            {
                var (messages, totalCount, participant) = await _teacherRepo.GetConversationMessagesAsync(
                    teacherUid,
                    conversationUid,
                    page,
                    limit,
                    before
                );

                if (participant == null)
                {
                    return new ConversationMessagesResponse
                    {
                        Success = false,
                        ConversationUid = conversationUid,
                        Participant = null,
                        Messages = new List<TeacherMessageDto>(),
                        Pagination = new MessagesPaginationDto { CurrentPage = page, HasMore = false }
                    };
                }

                return new ConversationMessagesResponse
                {
                    Success = true,
                    ConversationUid = conversationUid.ToString(),
                    Participant = participant,
                    Messages = messages,
                    Pagination = new MessagesPaginationDto
                    {
                        CurrentPage = page,
                        HasMore = (page * limit) < totalCount
                    }
                };
            }
            catch (Exception ex)
            {
                return new ConversationMessagesResponse
                {
                    Success = false,
                    ConversationUid = conversationUid.ToString(),
                    Participant = null,
                    Messages = new List<TeacherMessageDto>(),
                    Pagination = new MessagesPaginationDto { CurrentPage = page, HasMore = false }
                };
            }
        }

        public async Task<SendMessageResponse> SendMessageAsync(string teacherUid, string conversationUid, SendMessageRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return new SendMessageResponse
                    {
                        Success = false,
                        Message = "Nội dung tin nhắn không được để trống",
                        Data = null
                    };
                }

                var message = await _teacherRepo.SendMessageAsync(teacherUid, conversationUid, request.Content);

                if (message == null)
                {
                    return new SendMessageResponse
                    {
                        Success = false,
                        Message = "Không thể gửi tin nhắn",
                        Data = null
                    };
                }

                return new SendMessageResponse
                {
                    Success = true,
                    Message = "Gửi tin nhắn thành công",
                    Data = message
                };
            }
            catch (Exception ex)
            {
                return new SendMessageResponse
                {
                    Success = false,
                    Message = $"Lỗi khi gửi tin nhắn: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<MarkAsReadResponse> MarkAsReadAsync(string teacherUid, string conversationUid)
        {
            try
            {
                var success = await _teacherRepo.MarkAsReadAsync(teacherUid, conversationUid);

                if (!success)
                {
                    return new MarkAsReadResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy hội thoại"
                    };
                }

                return new MarkAsReadResponse
                {
                    Success = true,
                    Message = "Đánh dấu đã đọc thành công"
                };
            }
            catch (Exception ex)
            {
                return new MarkAsReadResponse
                {
                    Success = false,
                    Message = $"Lỗi khi đánh dấu đã đọc: {ex.Message}"
                };
            }
        }

        public async Task<TeacherDashboardStatsResponse> GetDashboardStatsAsync(string teacherUid)
        {
            try
            {
                var stats = await _teacherRepo.GetDashboardStatsAsync(teacherUid);

                return new TeacherDashboardStatsResponse
                {
                    Success = true,
                    Data = stats
                };
            }
            catch (Exception ex)
            {
                return new TeacherDashboardStatsResponse
                {
                    Success = false,
                    Data = new DashboardStatsDataDto()
                };
            }
        }
    }
}
