# Hướng dẫn Test Quiz Submission API

## 🎯 Endpoint cần test
```
POST https://localhost:7168/api/student/quiz-submissions
```

## 📝 Các thông tin bạn cần chuẩn bị

### 1. **JWT Token** (Bắt buộc)
- **Cách lấy**: Đăng nhập vào hệ thống qua endpoint `/api/auth/login`
- **Ví dụ login request**:
```http
POST https://localhost:7168/api/auth/login
Content-Type: application/json

{
  "email": "huy2@gmail.com",
  "password": "your-password"
}
```
- **Copy token từ response**: Lấy giá trị `token` trong response
- **Dán vào header**: `Authorization: Bearer <token-của-bạn>`

### 2. **Quiz UID** (Bắt buộc)
- **Cách lấy**: Gọi endpoint lấy danh sách quiz
```http
GET https://localhost:7168/api/student/quizzes
Authorization: Bearer <your-token>
```
- **Copy từ response**: Lấy giá trị `uid` của quiz bạn muốn nộp
- **Ví dụ**: `"3fa85f64-5717-4562-b3fc-2c963f66afa6"`

### 3. **Question UIDs và Answer UIDs**
- **Cách lấy**: Gọi endpoint lấy chi tiết quiz
```http
GET https://localhost:7168/api/student/quizzes/{quizUid}
Authorization: Bearer <your-token>
```
- **Response sẽ có dạng**:
```json
{
  "data": {
    "uid": "quiz-123",
    "title": "Linux Basics Quiz",
    "questions": [
      {
        "uid": "question-1",        // ← Copy cái này
        "content": "What is Linux?",
        "answers": [
          {
            "uid": "answer-1a",     // ← Copy cái này để chọn đáp án
            "content": "An operating system"
          },
          {
            "uid": "answer-1b",
            "content": "A programming language"
          }
        ]
      },
      {
        "uid": "question-2",        // ← Copy cái này
        "content": "What is sudo?",
        "answers": [
          {
            "uid": "answer-2a",     // ← Copy cái này
            "content": "Super user do"
          }
        ]
      }
    ]
  }
}
```

## 🚀 Các bước test cụ thể

### **Bước 1: Lấy Token**
```http
POST https://localhost:7168/api/auth/login
Content-Type: application/json

{
  "email": "huy2@gmail.com",
  "password": "123456"
}
```
**Lưu lại**: Token từ response

---

### **Bước 2: Lấy danh sách Quiz**
```http
GET https://localhost:7168/api/student/quizzes?courseSlug=linux
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
**Lưu lại**: Quiz UID (ví dụ: `"quiz-abc-123"`)

---

### **Bước 3: Xem chi tiết Quiz để lấy Questions & Answers**
```http
GET https://localhost:7168/api/student/quizzes/quiz-abc-123
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
**Lưu lại**: 
- Question 1 UID: `question-1`
- Answer 1a UID: `answer-1a`
- Question 2 UID: `question-2`  
- Answer 2a UID: `answer-2a`

---

### **Bước 4: Nộp Quiz**
```http
POST https://localhost:7168/api/student/quiz-submissions
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "quizUid": "quiz-abc-123",
  "answers": [
    {
      "questionUid": "question-1",
      "selectedAnswerUids": ["answer-1a"]
    },
    {
      "questionUid": "question-2",
      "selectedAnswerUids": ["answer-2a"]
    }
  ]
}
```

---

### **Bước 5: Xem kết quả**
```http
GET https://localhost:7168/api/student/quiz-submissions/{submissionUid}
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
(Lấy `submissionUid` từ response của Bước 4)

---

## 📋 Template điền sẵn để test

### Điền vào đây:
```
TOKEN: _______________________________________________

QUIZ_UID: _____________________________________________

QUESTION_1_UID: _______________________________________
ANSWER_1_UID: _________________________________________

QUESTION_2_UID: _______________________________________
ANSWER_2_UID: _________________________________________
```

### Request mẫu đã điền:
```http
POST https://localhost:7168/api/student/quiz-submissions
Authorization: Bearer [ĐIỀN TOKEN VÀO ĐÂY]
Content-Type: application/json

{
  "quizUid": "[ĐIỀN QUIZ_UID VÀO ĐÂY]",
  "answers": [
    {
      "questionUid": "[ĐIỀN QUESTION_1_UID VÀO ĐÂY]",
      "selectedAnswerUids": ["[ĐIỀN ANSWER_1_UID VÀO ĐÂY]"]
    },
    {
      "questionUid": "[ĐIỀN QUESTION_2_UID VÀO ĐÂY]",
      "selectedAnswerUids": ["[ĐIỀN ANSWER_2_UID VÀO ĐÂY]"]
    }
  ]
}
```

---

## ✅ Expected Response (Success)

```json
{
  "data": {
    "submissionUid": "submission-xyz-789",
    "score": 80.0,
    "correctCount": 4,
    "totalQuestions": 5,
    "startedAt": null,
    "submittedAt": "2025-11-05T10:30:00Z",
    "breakdown": [
      {
        "questionUid": "question-1",
        "isCorrect": true,
        "correctAnswerUids": ["answer-1a"]
      },
      {
        "questionUid": "question-2",
        "isCorrect": false,
        "correctAnswerUids": ["answer-2b"]
      }
    ]
  }
}
```

---

## ❌ Các lỗi thường gặp

### 1. **401 Unauthorized**
```json
{
  "error": {
    "message": "Unauthorized"
  }
}
```
**Nguyên nhân**: Token không hợp lệ hoặc hết hạn  
**Giải quyết**: Login lại để lấy token mới

---

### 2. **404 Not Found**
```json
{
  "error": {
    "code": 404,
    "message": "Quiz not found"
  }
}
```
**Nguyên nhân**: `quizUid` sai hoặc không tồn tại  
**Giải quyết**: Kiểm tra lại quizUid từ endpoint `/api/student/quizzes`

---

### 3. **422 Already Submitted**
```json
{
  "error": {
    "code": 422,
    "message": "You have already submitted this quiz"
  }
}
```
**Nguyên nhân**: User đã nộp quiz này rồi  
**Giải quyết**: Mỗi user chỉ được nộp 1 lần/quiz. Test với quiz khác hoặc user khác.

---

### 4. **422 Invalid Payload**
```json
{
  "error": {
    "code": 422,
    "message": "quizUid is required"
  }
}
```
**Nguyên nhân**: Thiếu `quizUid` trong request body  
**Giải quyết**: Đảm bảo có trường `quizUid` trong JSON

---

## 🧪 Test Cases bạn nên thử

### Test Case 1: Nộp bài đúng hoàn toàn
- Chọn tất cả đáp án đúng
- Kỳ vọng: `score = 100`, `correctCount = totalQuestions`

### Test Case 2: Nộp bài sai 1 câu
- Chọn 1 câu sai, còn lại đúng
- Kỳ vọng: `score = 80` (nếu 5 câu), `correctCount = 4`

### Test Case 3: Nộp bài trống
```json
{
  "quizUid": "quiz-abc-123",
  "answers": []
}
```
- Kỳ vọng: `score = 0`, `correctCount = 0`

### Test Case 4: Multiple choice (nhiều đáp án)
```json
{
  "questionUid": "question-3",
  "selectedAnswerUids": ["answer-3a", "answer-3b", "answer-3c"]
}
```
- Kỳ vọng: Câu chỉ đúng khi chọn **đủ** các đáp án đúng

### Test Case 5: Nộp lại quiz
- Nộp bài lần 1 → OK
- Nộp bài lần 2 với cùng quizUid → Error 422

---

## 🔍 Debug Tips

1. **Kiểm tra DB trước**: Đảm bảo có dữ liệu quiz, questions, answers trong DB
2. **Check IsCorrect**: Trong bảng `Answers`, cột `IsCorrect` phải có giá trị `true` cho đáp án đúng
3. **OrderIndex**: Questions nên có `OrderIndex` để hiển thị đúng thứ tự
4. **Quiz -> Lesson mapping**: Quiz phải liên kết với Lesson qua `LessonUid`

---

## 📌 Quick Reference

| Thông tin cần | Lấy từ đâu | Ví dụ |
|---------------|-----------|-------|
| Token | Login endpoint | `eyJhbGciOiJI...` |
| Quiz UID | GET `/api/student/quizzes` | `3fa85f64-5717...` |
| Question UID | GET `/api/student/quizzes/{quizUid}` | `question-1` |
| Answer UID | GET `/api/student/quizzes/{quizUid}` | `answer-1a` |
| Submission UID | Response của POST submission | `submission-xyz...` |

---

**Chúc bạn test thành công! 🎉**
