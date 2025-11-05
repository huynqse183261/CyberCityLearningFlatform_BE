# Student API Endpoints Documentation

## Overview
API endpoints dành cho học sinh (Student) trong hệ thống CyberCity Learning Platform.

**Base URL**: `/api/student`  
**Authentication**: Tất cả endpoints yêu cầu JWT Bearer token  
**Response Format**: 
- Success: `{ "data": {...} }`
- Error: `{ "error": { "code": number, "message": string, "details": object } }`

---

## 📚 Course Management

### 1. Get All Courses
Lấy danh sách tất cả các khóa học

**Endpoint**: `GET /api/student/courses`

**Query Parameters**:
- `category` (optional): Filter theo category/slug (ví dụ: `linux`, `pentest`)

**Response**:
```json
{
  "data": [
    {
      "uid": "string",
      "slug": "string",
      "title": "string",
      "description": "string",
      "coverImageUrl": null
    }
  ]
}
```

---

### 2. Get Course Outline
Lấy cấu trúc chi tiết của khóa học

**Endpoint**: `GET /api/student/courses/{slug}/outline`

**Path Parameters**:
- `slug`: Slug của khóa học (ví dụ: `linux`, `pentest`)

**Response**:
```json
{
  "data": {
    "course": {
      "uid": "string",
      "slug": "string",
      "title": "string",
      "description": "string"
    },
    "modules": [
      {
        "uid": "string",
        "title": "string",
        "orderIndex": 0,
        "lessons": [
          {
            "uid": "string",
            "title": "string",
            "orderIndex": 0,
            "topics": [
              {
                "uid": "string",
                "title": "string",
                "orderIndex": 0,
                "subtopics": [
                  {
                    "uid": "string",
                    "title": "string",
                    "orderIndex": 0
                  }
                ]
              }
            ]
          }
        ]
      }
    ]
  }
}
```

---

### 3. Enroll in Course
Đăng ký vào khóa học

**Endpoint**: `POST /api/student/courses/{courseUid}/enroll`

**Path Parameters**:
- `courseUid`: UID của khóa học

**Response**:
```json
{
  "data": {
    "message": "Enrollment successful",
    "courseUid": "string",
    "enrolledAt": "2025-11-05T10:30:00Z"
  }
}
```

---

### 4. Get My Enrollments
Lấy danh sách khóa học đã đăng ký

**Endpoint**: `GET /api/student/users/me/enrollments`

**Query Parameters**:
- `category` (optional): Filter theo category/slug

**Response**:
```json
{
  "data": [
    {
      "enrollmentUid": null,
      "courseUid": "string",
      "courseSlug": "string",
      "courseTitle": "string",
      "enrolledAt": "2025-11-05T10:30:00Z"
    }
  ]
}
```

---

## 📖 Learning Content (Theory)

### 5. Get Lesson Details
Lấy thông tin bài học và danh sách topics

**Endpoint**: `GET /api/student/lessons/{lessonUid}`

**Path Parameters**:
- `lessonUid`: UID của bài học

**Response**:
```json
{
  "data": {
    "uid": "string",
    "title": "string",
    "orderIndex": 0,
    "topics": [
      {
        "uid": "string",
        "title": "string",
        "orderIndex": 0
      }
    ]
  }
}
```

---

### 6. Get Topic Details
Lấy thông tin chủ đề và danh sách subtopics

**Endpoint**: `GET /api/student/topics/{topicUid}`

**Path Parameters**:
- `topicUid`: UID của topic

**Response**:
```json
{
  "data": {
    "uid": "string",
    "title": "string",
    "orderIndex": 0,
    "subtopics": [
      {
        "uid": "string",
        "title": "string",
        "orderIndex": 0
      }
    ]
  }
}
```

---

### 7. Get Subtopic Content
Lấy nội dung lý thuyết chi tiết

**Endpoint**: `GET /api/student/subtopics/{subtopicUid}`

**Path Parameters**:
- `subtopicUid`: UID của subtopic

**Response**:
```json
{
  "data": {
    "uid": "string",
    "title": "string",
    "orderIndex": 0,
    "contentHtml": "string"
  }
}
```

---

### 8. Update Subtopic Progress
Cập nhật tiến độ học lý thuyết

**Endpoint**: `POST /api/student/subtopics/{subtopicUid}/progress`

**Path Parameters**:
- `subtopicUid`: UID của subtopic

**Request Body**:
```json
{
  "progress": 100
}
```

**Response**:
```json
{
  "data": {
    "subtopicUid": "string",
    "progress": 100,
    "updatedAt": "2025-11-05T10:30:00Z"
  }
}
```

---

## 📝 Quizzes

### 9. Get Quizzes List
Lấy danh sách bài quiz

**Endpoint**: `GET /api/student/quizzes`

**Query Parameters**:
- `courseSlug` (optional): Filter theo slug của course
- `moduleUid` (optional): Filter theo UID của module
- `lessonUid` (optional): Filter theo UID của lesson

**Response**:
```json
{
  "data": [
    {
      "uid": "string",
      "title": "string",
      "description": "string",
      "lessonUid": "string",
      "moduleUid": "string",
      "numQuestions": 10,
      "timeLimitSeconds": null
    }
  ]
}
```

---

### 10. Get Quiz Details
Lấy chi tiết quiz (câu hỏi + đáp án, KHÔNG có isCorrect)

**Endpoint**: `GET /api/student/quizzes/{quizUid}`

**Path Parameters**:
- `quizUid`: UID của quiz

**Response**:
```json
{
  "data": {
    "uid": "string",
    "title": "string",
    "description": "string",
    "timeLimitSeconds": null,
    "questions": [
      {
        "uid": "string",
        "content": "string",
        "orderIndex": 0,
        "multipleChoice": true,
        "answers": [
          {
            "uid": "string",
            "content": "string"
          }
        ]
      }
    ]
  }
}
```

**⚠️ Lưu ý**: Đáp án đúng (`isCorrect`) KHÔNG được trả về trong endpoint này để tránh gian lận.

---

### 11. Submit Quiz
Nộp bài quiz

**Endpoint**: `POST /api/student/quiz-submissions`

**Request Body**:
```json
{
  "quizUid": "string",
  "answers": [
    {
      "questionUid": "string",
      "selectedAnswerUids": ["string"]
    }
  ]
}
```

**Response**:
```json
{
  "data": {
    "submissionUid": "string",
    "quizUid": "string",
    "score": 80.0,
    "totalQuestions": 10,
    "correctCount": 8,
    "submittedAt": "2025-11-05T10:30:00Z"
  }
}
```

---

### 12. Get Quiz Submission Result
Xem kết quả bài quiz đã nộp

**Endpoint**: `GET /api/student/quiz-submissions/{submissionUid}`

**Path Parameters**:
- `submissionUid`: UID của submission

**Response**:
```json
{
  "data": {
    "submissionUid": "string",
    "quizUid": "string",
    "quizTitle": "string",
    "score": 80.0,
    "correctCount": 8,
    "totalQuestions": 10,
    "startedAt": null,
    "submittedAt": "2025-11-05T10:30:00Z",
    "breakdown": [
      {
        "questionUid": "string",
        "questionContent": "string",
        "isCorrect": true,
        "selectedAnswerUids": ["string"],
        "correctAnswerUids": ["string"]
      }
    ]
  }
}
```

---

## 📊 Progress Tracking

### 13. Get Course Progress
Xem tổng quan tiến độ khóa học

**Endpoint**: `GET /api/student/users/me/progress/courses/{courseUid}`

**Path Parameters**:
- `courseUid`: UID của khóa học

**Response**:
```json
{
  "data": {
    "courseUid": "string",
    "courseTitle": "string",
    "completedSubtopics": 15,
    "totalSubtopics": 20,
    "progressPercentage": 75,
    "quizzes": {
      "completed": 3,
      "total": 5,
      "averageScore": 85.5
    }
  }
}
```

---

### 14. Get Lesson Progress
Xem tiến độ chi tiết của bài học

**Endpoint**: `GET /api/student/users/me/progress/lessons/{lessonUid}`

**Path Parameters**:
- `lessonUid`: UID của bài học

**Response**:
```json
{
  "data": {
    "lessonUid": "string",
    "lessonTitle": "string",
    "completedSubtopics": 5,
    "totalSubtopics": 8,
    "progressPercentage": 62,
    "quiz": {
      "quizUid": "string",
      "completed": true,
      "score": 90.0,
      "lastAttempt": "2025-11-05T10:30:00Z"
    }
  }
}
```

---

## 🔒 Authorization

Tất cả endpoints yêu cầu JWT token trong header:

```
Authorization: Bearer <your-jwt-token>
```

Token phải chứa claim `uid` hoặc `NameIdentifier` để xác định user hiện tại.

---

## ⚠️ Error Responses

### Common Error Codes:
- **401 Unauthorized**: Thiếu hoặc token không hợp lệ
- **404 Not Found**: Resource không tồn tại
- **422 Unprocessable Entity**: Dữ liệu request không hợp lệ
- **500 Internal Server Error**: Lỗi server

### Error Response Format:
```json
{
  "error": {
    "code": 404,
    "message": "Course not found",
    "details": null
  }
}
```

---

## 📌 Implementation Notes

1. **Slug Strategy**: Sử dụng trường `Slug` từ database thay vì tự động tạo từ title
2. **Quiz Security**: Đáp án đúng chỉ được trả về sau khi submit quiz
3. **Progress Calculation**: 
   - Subtopic được tính là hoàn thành khi `progress >= 100`
   - Quiz được tính là hoàn thành khi có submission
4. **Date Format**: Tất cả datetime theo chuẩn ISO 8601 (UTC)

---

**Last Updated**: November 5, 2025  
**Controller**: `StudentApiController.cs`  
**Base Route**: `/api/student`
