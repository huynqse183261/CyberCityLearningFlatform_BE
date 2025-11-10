# API Xem Đơn Hàng và Hóa Đơn từ Sepay

## Tổng quan
Document này hướng dẫn sử dụng các API để xem đơn hàng (orders) và hóa đơn (invoices) từ hệ thống thanh toán Sepay.

---

## 🔐 Authentication
Tất cả các endpoint yêu cầu JWT token trong header:
```
Authorization: Bearer <your_jwt_token>
```

---

## 📋 API Endpoints

### 1. Lấy Tất Cả Đơn Hàng (Admin Only)

**Endpoint:** `GET /api/payment/orders`

**Authorization:** Admin role required

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "uid": "order-uid-123",
      "userName": "Nguyễn Văn A",
      "userEmail": "nguyenvana@example.com",
      "planName": "Premium Plan",
      "amount": 299000,
      "paymentStatus": "paid",
      "approvalStatus": "approved",
      "createdAt": "2025-11-10T10:30:00Z",
      "paidAt": "2025-11-10T10:35:00Z",
      "paymentCount": 1
    }
  ],
  "total": 1
}
```

**Ví dụ sử dụng:**
```bash
curl -X GET "https://cybercitylearningflatform-be.onrender.com/api/payment/orders" \
  -H "Authorization: Bearer <admin_token>"
```

---

### 2. Lấy Đơn Hàng Của User

**Endpoint:** `GET /api/payment/orders/user/{userUid}`

**Authorization:** Authenticated user

**Parameters:**
- `userUid` (path): UID của user

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "uid": "order-uid-123",
      "userName": "Nguyễn Văn A",
      "userEmail": "nguyenvana@example.com",
      "planName": "Premium Plan",
      "amount": 299000,
      "paymentStatus": "paid",
      "approvalStatus": "approved",
      "createdAt": "2025-11-10T10:30:00Z",
      "paidAt": "2025-11-10T10:35:00Z",
      "paymentCount": 1
    }
  ],
  "total": 1
}
```

**Ví dụ sử dụng:**
```bash
curl -X GET "https://cybercitylearningflatform-be.onrender.com/api/payment/orders/user/user-uid-123" \
  -H "Authorization: Bearer <user_token>"
```

---

### 3. Lấy Chi Tiết Đơn Hàng

**Endpoint:** `GET /api/payment/order/{orderUid}`

**Authorization:** Authenticated user

**Parameters:**
- `orderUid` (path): UID của order

**Response:**
```json
{
  "success": true,
  "data": {
    "uid": "order-uid-123",
    "userUid": "user-uid-123",
    "userName": "Nguyễn Văn A",
    "userEmail": "nguyenvana@example.com",
    "orgUid": null,
    "orgName": null,
    "planUid": "plan-uid-123",
    "planName": "Premium Plan",
    "durationDays": 30,
    "amount": 299000,
    "paymentStatus": "paid",
    "approvalStatus": "approved",
    "startAt": "2025-11-10T10:35:00Z",
    "endAt": "2025-12-10T10:35:00Z",
    "createdAt": "2025-11-10T10:30:00Z",
    "payments": [
      {
        "uid": "payment-uid-123",
        "paymentMethod": "SEPAY",
        "transactionCode": "ORD56603b13-1470db2f",
        "amount": 299000,
        "currency": "VND",
        "status": "paid",
        "paidAt": "2025-11-10T10:35:00Z",
        "createdAt": "2025-11-10T10:30:00Z"
      }
    ]
  }
}
```

**Ví dụ sử dụng:**
```bash
curl -X GET "https://cybercitylearningflatform-be.onrender.com/api/payment/order/order-uid-123" \
  -H "Authorization: Bearer <user_token>"
```

---

### 4. Lấy Hóa Đơn Chi Tiết

**Endpoint:** `GET /api/payment/invoice/{paymentUid}`

**Authorization:** Authenticated user

**Parameters:**
- `paymentUid` (path): UID của payment

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentUid": "payment-uid-123",
    "invoiceNumber": "INV-ORD56603b13-1470db2f",
    "invoiceDate": "2025-11-10T10:30:00Z",
    "customerName": "Nguyễn Văn A",
    "customerEmail": "nguyenvana@example.com",
    "customerPhone": "",
    "orderUid": "order-uid-123",
    "planName": "Premium Plan",
    "durationDays": 30,
    "serviceStartDate": "2025-11-10T10:35:00Z",
    "serviceEndDate": "2025-12-10T10:35:00Z",
    "paymentMethod": "SEPAY",
    "transactionCode": "ORD56603b13-1470db2f",
    "amount": 299000,
    "currency": "VND",
    "status": "paid",
    "paidAt": "2025-11-10T10:35:00Z",
    "organizationName": null,
    "organizationCode": ""
  }
}
```

**Ví dụ sử dụng:**
```bash
curl -X GET "https://cybercitylearningflatform-be.onrender.com/api/payment/invoice/payment-uid-123" \
  -H "Authorization: Bearer <user_token>"
```

---

### 5. Lấy Lịch Sử Thanh Toán

**Endpoint:** `GET /api/payment/history/{userUid}`

**Authorization:** Authenticated user

**Parameters:**
- `userUid` (path): UID của user

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "uid": "payment-uid-123",
      "orderId": "order-uid-123",
      "amount": 299000,
      "currency": "VND",
      "paymentMethod": "SEPAY",
      "status": "paid",
      "description": "",
      "transactionId": "ORD56603b13-1470db2f",
      "createdAt": "2025-11-10T10:30:00Z",
      "completedAt": "2025-11-10T10:35:00Z"
    }
  ]
}
```

**Ví dụ sử dụng:**
```bash
curl -X GET "https://cybercitylearningflatform-be.onrender.com/api/payment/history/user-uid-123" \
  -H "Authorization: Bearer <user_token>"
```

---

## 📊 Payment Status Values

| Status | Mô tả |
|--------|-------|
| `pending` | Đang chờ thanh toán |
| `paid` | Đã thanh toán thành công |
| `completed` | Đã hoàn tất |
| `failed` | Thanh toán thất bại |
| `cancelled` | Đã hủy |

## 📊 Approval Status Values

| Status | Mô tả |
|--------|-------|
| `pending` | Đang chờ duyệt |
| `approved` | Đã duyệt |
| `rejected` | Bị từ chối |

---

## 🔍 Use Cases

### Use Case 1: Admin xem tất cả đơn hàng
1. Admin đăng nhập và lấy JWT token
2. Gọi `GET /api/payment/orders` với admin token
3. Xem danh sách tất cả đơn hàng trong hệ thống

### Use Case 2: User xem đơn hàng của mình
1. User đăng nhập và lấy JWT token
2. Gọi `GET /api/payment/orders/user/{userUid}`
3. Xem danh sách đơn hàng của chính mình

### Use Case 3: Xem chi tiết đơn hàng và các payment
1. Từ danh sách đơn hàng, lấy `orderUid`
2. Gọi `GET /api/payment/order/{orderUid}`
3. Xem chi tiết order bao gồm tất cả payments liên quan

### Use Case 4: In hóa đơn
1. Từ chi tiết order, lấy `paymentUid` của payment đã paid
2. Gọi `GET /api/payment/invoice/{paymentUid}`
3. Hiển thị/in hóa đơn chi tiết

---

## ⚠️ Error Responses

```json
{
  "success": false,
  "message": "Order with UID xxx not found"
}
```

```json
{
  "success": false,
  "message": "Unauthorized"
}
```

---

## 🔗 Related APIs

- [Payment API Documentation](./PAYMENT_API_DOCUMENTATION.md)
- [Sepay Webhook Testing](./SEPAY_WEBHOOK_TESTING.md)
- [Payment Cancel Flow](./PAYMENT_CANCEL_FLOW.md)

---

## 📝 Notes

1. **Authorization**: Endpoint `/api/payment/orders` chỉ dành cho Admin. Các endpoint khác yêu cầu user đã đăng nhập.
2. **Payment Count**: Mỗi order có thể có nhiều payment (ví dụ: payment bị failed và retry).
3. **Paid Date**: `paidAt` là thời điểm payment đầu tiên có status `paid` hoặc `completed`.
4. **Transaction Code**: Là mã giao dịch duy nhất để tra cứu trên Sepay, format: `ORD{8chars}-{8chars}`.

---

**Last Updated:** November 10, 2025
