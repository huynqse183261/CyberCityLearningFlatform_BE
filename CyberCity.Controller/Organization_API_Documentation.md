# Organization API Documentation

## Tổng quan
API này cung cấp các chức năng quản lý tổ chức và thành viên tổ chức trong hệ thống CyberCity.

## Base URL
```
/api/organizations
```

## Endpoints

### 1. Lấy danh sách tổ chức
```
GET /api/organizations
```

**Query Parameters:**
- `pageNumber` (int, optional): Số trang (mặc định: 1)
- `pageSize` (int, optional): Số lượng item mỗi trang (mặc định: 10)
- `descending` (bool, optional): Sắp xếp giảm dần (mặc định: true)

**Response:**
```json
{
  "items": [
    {
      "uid": "guid",
      "orgName": "string",
      "orgType": "string",
      "contactEmail": "string",
      "createdAt": "datetime",
      "memberCount": 0
    }
  ],
  "totalItems": 0,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 0
}
```

### 2. Lấy chi tiết tổ chức
```
GET /api/organizations/{id}
```

**Path Parameters:**
- `id` (Guid): ID của tổ chức

**Response:**
```json
{
  "uid": "guid",
  "orgName": "string",
  "orgType": "string",
  "contactEmail": "string",
  "createdAt": "datetime",
  "memberCount": 0
}
```

### 3. Tạo tổ chức mới
```
POST /api/organizations
```

**Request Body:**
```json
{
  "orgName": "string (required, max 255)",
  "orgType": "string (max 50)",
  "contactEmail": "string (email format, max 255)"
}
```

**Response:**
```json
{
  "uid": "guid",
  "orgName": "string",
  "orgType": "string",
  "contactEmail": "string",
  "createdAt": "datetime",
  "memberCount": 0
}
```

### 4. Cập nhật tổ chức
```
PUT /api/organizations/{id}
```

**Path Parameters:**
- `id` (Guid): ID của tổ chức

**Request Body:**
```json
{
  "orgName": "string (required, max 255)",
  "orgType": "string (max 50)",
  "contactEmail": "string (email format, max 255)"
}
```

**Response:**
```json
{
  "uid": "guid",
  "orgName": "string",
  "orgType": "string",
  "contactEmail": "string",
  "createdAt": "datetime",
  "memberCount": 0
}
```

### 5. Xóa tổ chức
```
DELETE /api/organizations/{id}
```

**Path Parameters:**
- `id` (Guid): ID của tổ chức

**Response:** 204 No Content

### 6. Lấy danh sách thành viên của tổ chức
```
GET /api/organizations/{id}/members
```

**Path Parameters:**
- `id` (Guid): ID của tổ chức

**Query Parameters:**
- `pageNumber` (int, optional): Số trang (mặc định: 1)
- `pageSize` (int, optional): Số lượng item mỗi trang (mặc định: 10)

**Response:**
```json
{
  "items": [
    {
      "uid": "guid",
      "orgUid": "guid",
      "userUid": "guid",
      "memberRole": "string",
      "joinedAt": "datetime",
      "userFullName": "string",
      "userEmail": "string",
      "userUsername": "string",
      "userImage": "string"
    }
  ],
  "totalItems": 0,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 0
}
```

### 7. Thêm thành viên vào tổ chức
```
POST /api/organizations/{id}/members
```

**Path Parameters:**
- `id` (Guid): ID của tổ chức

**Request Body:**
```json
{
  "userUid": "guid (required)",
  "memberRole": "string (required, max 50)"
}
```

**Response:**
```json
{
  "uid": "guid",
  "orgUid": "guid",
  "userUid": "guid",
  "memberRole": "string",
  "joinedAt": "datetime",
  "userFullName": "string",
  "userEmail": "string",
  "userUsername": "string",
  "userImage": "string"
}
```

### 8. Cập nhật vai trò thành viên
```
PUT /api/organizations/{id}/members/{userId}/role
```

**Path Parameters:**
- `id` (Guid): ID của tổ chức
- `userId` (Guid): ID của user

**Request Body:**
```json
{
  "memberRole": "string (required, max 50)"
}
```

**Response:**
```json
{
  "uid": "guid",
  "orgUid": "guid",
  "userUid": "guid",
  "memberRole": "string",
  "joinedAt": "datetime",
  "userFullName": "string",
  "userEmail": "string",
  "userUsername": "string",
  "userImage": "string"
}
```

### 9. Xóa thành viên khỏi tổ chức
```
DELETE /api/organizations/{id}/members/{userId}
```

**Path Parameters:**
- `id` (Guid): ID của tổ chức
- `userId` (Guid): ID của user

**Response:** 204 No Content

## Error Responses

### 400 Bad Request
```json
{
  "message": "Validation error message"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "Error message",
  "error": "Detailed error information"
}
```

## Validation Rules

### Organization
- `orgName`: Bắt buộc, tối đa 255 ký tự
- `orgType`: Tối đa 50 ký tự
- `contactEmail`: Định dạng email hợp lệ, tối đa 255 ký tự

### Organization Member
- `userUid`: Bắt buộc, phải là GUID hợp lệ
- `memberRole`: Bắt buộc, tối đa 50 ký tự

## Business Rules

1. **Tạo tổ chức**: Tự động tạo UID và thời gian tạo
2. **Thêm thành viên**: Kiểm tra user đã là thành viên chưa trước khi thêm
3. **Xóa tổ chức**: Xóa tất cả thành viên liên quan
4. **Cập nhật vai trò**: Chỉ cập nhật vai trò của thành viên hiện có

## Notes

- Tất cả thời gian được trả về theo định dạng UTC
- Pagination được áp dụng cho các endpoint danh sách
- API hỗ trợ CORS cho frontend React
- JWT Authentication có thể được áp dụng cho các endpoint nhạy cảm
