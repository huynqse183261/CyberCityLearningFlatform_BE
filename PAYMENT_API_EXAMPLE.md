# PayOS Payment Integration - API Documentation

## Tổng quan
API tích hợp PayOS để xử lý thanh toán cho hệ thống CyberCity Learning Platform.

## Flow thanh toán

1. **Client gửi request** với `UserUid` + `PlanUid`
2. **Server tự động**:
   - Lấy thông tin User từ database
   - Lấy thông tin Pricing Plan từ database (bao gồm giá tiền)
   - Tạo Order mới với trạng thái `pending`
   - Tạo Payment Link trên PayOS
   - Lưu Payment record vào database
3. **Server trả về** link thanh toán + QR code
4. **User thanh toán** qua PayOS
5. **PayOS gửi webhook** về server
6. **Server cập nhật** trạng thái Order và Payment

---

## API Endpoints

### 1. Tạo Link Thanh Toán

**Endpoint:** `POST /api/payment/create-payment-link`

**Authorization:** Bearer Token (Required)

**Request Body:**
```json
{
  "userUid": "550e8400-e29b-41d4-a716-446655440000",
  "planUid": "660e8400-e29b-41d4-a716-446655440001",
  "orgUid": "770e8400-e29b-41d4-a716-446655440002",  // Optional - chỉ cần khi mua gói cho organization
  "returnUrl": "https://yourapp.com/payment/success",
  "cancelUrl": "https://yourapp.com/payment/cancel"
}
```

**Response Success (200 OK):**
```json
{
  "success": true,
  "data": {
    "uid": "880e8400-e29b-41d4-a716-446655440003",
    "checkoutUrl": "https://pay.payos.vn/web/xxxxxxxxxxxx",
    "qrCode": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA...",
    "orderCode": 1730678400123,
    "status": "pending",
    "amount": 299000,
    "description": "Nguyễn Văn A - Gói Premium (30 ngày)",
    "userName": "Nguyễn Văn A",
    "planName": "Gói Premium"
  },
  "message": "Tạo link thanh toán thành công"
}
```

**Response Error (400 Bad Request):**
```json
{
  "success": false,
  "message": "User with UID xxx not found"
}
```

---

### 2. Kiểm Tra Trạng Thái Thanh Toán

**Endpoint:** `GET /api/payment/status/{orderCode}`

**Authorization:** Bearer Token (Required)

**Parameters:**
- `orderCode` (path parameter): Mã đơn hàng PayOS (long integer)

**Response Success (200 OK):**
```json
{
  "success": true,
  "data": {
    "orderCode": 1730678400123,
    "amount": 299000,
    "amountPaid": "299000",
    "amountRemaining": 0,
    "status": "PAID",
    "createdAt": "2024-11-03T10:30:00Z",
    "canceledAt": null,
    "cancellationReason": null
  }
}
```

---

### 3. Webhook từ PayOS

**Endpoint:** `POST /api/payment/webhook`

**Authorization:** None (Public endpoint for PayOS)

**Request Body:** (Tự động gửi từ PayOS)
```json
{
  "code": "00",
  "desc": "Thành công",
  "data": {
    "orderCode": 1730678400123,
    "amount": 299000,
    "description": "Nguyễn Văn A - Gói Premium (30 ngày)",
    "accountNumber": "12345678",
    "reference": "FT12345678",
    "transactionDateTime": "2024-11-03T10:35:00Z",
    "currency": "VND",
    "paymentLinkId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "code": "00",
    "desc": "Thành công",
    "counterAccountBankId": "",
    "counterAccountBankName": "",
    "counterAccountName": "",
    "counterAccountNumber": "",
    "virtualAccountName": "",
    "virtualAccountNumber": ""
  },
  "signature": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Payment webhook processed successfully"
}
```

---

### 4. Hủy Link Thanh Toán

**Endpoint:** `POST /api/payment/cancel/{orderCode}`

**Authorization:** Bearer Token (Required)

**Parameters:**
- `orderCode` (path parameter): Mã đơn hàng PayOS

**Request Body:**
```json
{
  "cancellationReason": "Khách hàng hủy đơn hàng"
}
```

**Response Success (200 OK):**
```json
{
  "success": true,
  "message": "Payment link cancelled successfully"
}
```

---

## Database Schema

### Order Table
```sql
- uid: string (PK)
- user_uid: string (FK -> User)
- org_uid: string (FK -> Organization, nullable)
- plan_uid: string (FK -> PricingPlan)
- amount: decimal (Tự động lấy từ PricingPlan.Price)
- payment_status: string (pending/paid/failed/cancelled)
- approval_status: string (pending/approved/rejected)
- start_at: datetime (nullable)
- end_at: datetime (nullable)
- created_at: datetime
```

### Payment Table
```sql
- uid: string (PK)
- order_uid: string (FK -> Order)
- payment_method: string (PayOS)
- transaction_code: string (PayOS order code)
- amount: decimal
- currency: string (VND)
- status: string (pending/completed/failed/cancelled)
- paid_at: datetime (nullable)
- created_at: datetime
```

### PricingPlan Table
```sql
- uid: string (PK)
- plan_name: string
- price: decimal (Giá gói dịch vụ)
- duration_days: int
- features: string
- created_at: datetime
```

---

## Frontend Integration Example

### React/TypeScript Example

```typescript
interface CreatePaymentRequest {
  userUid: string;
  planUid: string;
  orgUid?: string;
  returnUrl: string;
  cancelUrl: string;
}

const createPayment = async (userId: string, planId: string) => {
  const request: CreatePaymentRequest = {
    userUid: userId,
    planUid: planId,
    returnUrl: `${window.location.origin}/payment/success`,
    cancelUrl: `${window.location.origin}/payment/cancel`
  };

  try {
    const response = await fetch('/api/payment/create-payment-link', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
      },
      body: JSON.stringify(request)
    });

    const result = await response.json();
    
    if (result.success) {
      // Redirect to payment page
      window.location.href = result.data.checkoutUrl;
      
      // Or show QR code
      // setQrCode(result.data.qrCode);
    }
  } catch (error) {
    console.error('Payment error:', error);
  }
};
```

---

## Testing với Postman

### 1. Tạo Payment Link

```bash
POST http://localhost:5000/api/payment/create-payment-link
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "userUid": "YOUR_USER_UID",
  "planUid": "YOUR_PLAN_UID",
  "returnUrl": "http://localhost:5173/payment/success",
  "cancelUrl": "http://localhost:5173/payment/cancel"
}
```

### 2. Kiểm tra Status

```bash
GET http://localhost:5000/api/payment/status/1730678400123
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## Lưu ý quan trọng

### ✅ Ưu điểm của cách thiết kế này:

1. **Đơn giản hóa Request**: Frontend chỉ cần gửi UserUid + PlanUid
2. **Tự động tính giá**: Server tự động lấy giá từ PricingPlan
3. **Bảo mật**: Không cho phép client tự set giá tiền
4. **Thông tin đầy đủ**: Description tự động kết hợp tên user + tên gói
5. **Dễ maintain**: Thay đổi giá chỉ cần update trong PricingPlan table

### ⚠️ Xử lý lỗi:

- User không tồn tại → `User with UID xxx not found`
- Plan không tồn tại → `Pricing plan with UID xxx not found`
- PayOS API error → `Failed to create payment link: [error message]`

### 🔒 Security:

- Tất cả endpoints (trừ webhook) yêu cầu JWT token
- Amount được lấy từ database, không cho phép client tự set
- Webhook cần verify signature từ PayOS

---

## PayOS Configuration

Trong `appsettings.json`:

```json
{
  "PayOS": {
    "ClientId": "db541eb3-2b5b-4892-8344-8bd115f7f8f4",
    "ApiKey": "your-api-key-here",
    "ChecksumKey": "your-checksum-key-here"
  }
}
```

## Support

- PayOS Documentation: https://payos.vn/docs/api/
- PayOS Test Environment: https://payos.vn/
