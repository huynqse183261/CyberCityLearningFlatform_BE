# 📧 API HỘP THƯ ADMIN - CYBERCITY LEARNING PLATFORM

## 📝 TỔNG QUAN

API quản lý tin nhắn cho Admin với 5 endpoints cơ bản:
1. ✅ Lấy danh sách cuộc hội thoại
2. ✅ Lấy tin nhắn trong cuộc hội thoại
3. ✅ Gửi tin nhắn phản hồi
4. ✅ Xóa tin nhắn
5. ✅ Thống kê tổng quan

---

## 🚀 CÁC API ĐÃ IMPLEMENT

### 1️⃣ Lấy danh sách cuộc hội thoại

**Endpoint:**
```http
GET /api/admin/conversations
```

**Headers:**
```
Authorization: Bearer {JWT_TOKEN}
```

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| PageNumber | int | 1 | Số trang |
| PageSize | int | 20 | Số items mỗi trang |
| SearchQuery | string | null | Tìm kiếm theo tên cuộc hội thoại |

**Response Example:**
```json
{
  "items": [
    {
      "uid": "123e4567-e89b-12d3-a456-426614174000",
      "title": "Hỗ trợ khóa học Python",
      "isGroup": false,
      "totalMessages": 25,
      "createdAt": "2025-10-01T10:00:00Z",
      "lastMessageAt": "2025-10-14T08:30:00Z",
      "members": [
        {
          "uid": "user-123",
          "username": "student01",
          "fullName": "Nguyễn Văn A",
          "role": "student",
          "image": "https://example.com/avatar.jpg"
        }
      ]
    }
  ],
  "totalItems": 150,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 8
}
```

---

### 2️⃣ Lấy tin nhắn trong cuộc hội thoại

**Endpoint:**
```http
GET /api/admin/conversations/{conversationId}/messages
```

**Headers:**
```
Authorization: Bearer {JWT_TOKEN}
```

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| conversationId | Guid | ID của cuộc hội thoại |

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| PageNumber | int | 1 | Số trang |
| PageSize | int | 50 | Số items mỗi trang |

**Response Example:**
```json
{
  "items": [
    {
      "uid": "msg-123",
      "conversationUid": "conv-456",
      "senderUid": "user-789",
      "message": "Xin chào, tôi cần hỗ trợ",
      "sentAt": "2025-10-14T08:30:00Z",
      "sender": {
        "uid": "user-789",
        "username": "student01",
        "fullName": "Nguyễn Văn A",
        "role": "student",
        "image": null
      }
    }
  ],
  "totalItems": 25,
  "pageNumber": 1,
  "pageSize": 50,
  "totalPages": 1
}
```

---

### 3️⃣ Gửi tin nhắn phản hồi

**Endpoint:**
```http
POST /api/admin/conversations/{conversationId}/messages
```

**Headers:**
```
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json
```

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| conversationId | Guid | ID của cuộc hội thoại |

**Request Body:**
```json
{
  "message": "Xin chào, tôi là admin. Tôi có thể giúp gì cho bạn?"
}
```

**Response Example:**
```json
{
  "isSuccess": true,
  "message": "Message sent successfully",
  "data": {
    "uid": "msg-new-123",
    "conversationUid": "conv-456",
    "senderUid": "admin-001",
    "message": "Xin chào, tôi là admin. Tôi có thể giúp gì cho bạn?",
    "sentAt": "2025-10-14T09:00:00Z",
    "sender": {
      "uid": "admin-001",
      "username": "admin",
      "fullName": "Admin Hệ Thống",
      "role": "admin",
      "image": null
    }
  }
}
```

---

### 4️⃣ Xóa tin nhắn

**Endpoint:**
```http
DELETE /api/admin/messages/{messageId}
```

**Headers:**
```
Authorization: Bearer {JWT_TOKEN}
```

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| messageId | Guid | ID của tin nhắn cần xóa |

**Response Example:**
```json
{
  "isSuccess": true,
  "message": "Message deleted successfully"
}
```

**Error Response:**
```json
{
  "isSuccess": false,
  "message": "Message not found"
}
```

---

### 5️⃣ Thống kê tổng quan

**Endpoint:**
```http
GET /api/admin/messages/stats
```

**Headers:**
```
Authorization: Bearer {JWT_TOKEN}
```

**Response Example:**
```json
{
  "totalConversations": 150,
  "totalMessages": 3420,
  "todayMessages": 45,
  "thisWeekMessages": 287
}
```

---

## 🔐 PHÂN QUYỀN

Tất cả các API đều yêu cầu:
- ✅ **Authentication**: Bearer JWT Token
- ✅ **Authorization**: Role = `admin`

Nếu không phải admin, API sẽ trả về:
```json
{
  "status": 403,
  "message": "Forbidden"
}
```

---

## 📂 CẤU TRÚC CODE ĐÃ TẠO

```
CyberCity.DTOs/Admin/
├── GetConversationsQuery.cs
├── GetMessagesQuery.cs
├── SimpleUserDto.cs
├── ConversationDto.cs
├── ConversationsListResponse.cs
├── MessageDto.cs
├── MessagesListResponse.cs
├── SendMessageRequest.cs
├── SendMessageResponse.cs
├── DeleteMessageResponse.cs
└── MessageStatsResponse.cs

CyberCity.Application/
├── Interface/
│   └── IAdminMessageService.cs
└── Implement/
    └── AdminMessageService.cs

CyberCity.Infrastructure/
└── AdminMessageRepo.cs (implements IAdminMessageRepository)

CyberCity.Controller/Controllers/
└── AdminMessagesController.cs
```

---

## 🧪 TESTING VỚI SWAGGER

1. Chạy ứng dụng
2. Truy cập: `https://localhost:{port}/swagger`
3. Authenticate với JWT token của admin
4. Test các endpoints

---

## 📱 INTEGRATION VỚI FRONTEND

### TypeScript Service Example:

```typescript
import axios from 'axios';

class AdminMessageService {
  private baseURL = '/api/admin';

  // 1. Get conversations
  async getConversations(params: {
    pageNumber?: number;
    pageSize?: number;
    searchQuery?: string;
  }) {
    const response = await axios.get(`${this.baseURL}/conversations`, { params });
    return response.data;
  }

  // 2. Get messages
  async getMessages(conversationId: string, params: {
    pageNumber?: number;
    pageSize?: number;
  }) {
    const response = await axios.get(
      `${this.baseURL}/conversations/${conversationId}/messages`,
      { params }
    );
    return response.data;
  }

  // 3. Send message
  async sendMessage(conversationId: string, message: string) {
    const response = await axios.post(
      `${this.baseURL}/conversations/${conversationId}/messages`,
      { message }
    );
    return response.data;
  }

  // 4. Delete message
  async deleteMessage(messageId: string) {
    const response = await axios.delete(`${this.baseURL}/messages/${messageId}`);
    return response.data;
  }

  // 5. Get stats
  async getStats() {
    const response = await axios.get(`${this.baseURL}/messages/stats`);
    return response.data;
  }
}

export default new AdminMessageService();
```

---

## 🗄️ DATABASE TABLES SỬ DỤNG

1. **conversations** - Cuộc hội thoại
2. **messages** - Tin nhắn
3. **conversation_members** - Thành viên cuộc hội thoại
4. **users** - Người dùng

---

## ✅ CHECKLIST

- [x] DTOs đã tạo
- [x] Repository đã implement
- [x] Service đã implement
- [x] Controller đã tạo
- [x] Dependency Injection đã đăng ký
- [x] Authorization với role admin
- [x] Phân trang cho tất cả list endpoints
- [x] Tìm kiếm cuộc hội thoại
- [x] Validation cho request body
- [x] Error handling

---

## 🎯 NEXT STEPS (Tùy chọn)

- [ ] Thêm filter nâng cao (theo role, theo ngày tháng)
- [ ] Đánh dấu đã đọc/chưa đọc
- [ ] Notification khi có tin nhắn mới
- [ ] Export cuộc hội thoại ra file
- [ ] Tìm kiếm nội dung tin nhắn

---

> **Lưu ý:** 
> - Tất cả datetime đều sử dụng UTC
> - Hỗ trợ phân trang cho performance tốt hơn
> - Sử dụng EF Core LINQ với PostgreSQL
> - Code đã được tối ưu cho production
