# Sepay Payment Integration - API Documentation

## Tổng quan
API tích hợp Sepay để xử lý thanh toán qua QR Code cho hệ thống CyberCity Learning Platform.

## Flow thanh toán

1. **Client gửi request** với `UserUid` + `PlanUid`
2. **Server tự động**:
   - Lấy thông tin User từ database
   - Lấy thông tin Pricing Plan từ database (bao gồm giá tiền)
   - Tạo Order mới với trạng thái `pending`
   - Tạo QR Code URL từ Sepay (format: `https://qr.sepay.vn/img?acc={accountNumber}&bank={bankCode}&amount={amount}&des={description}`)
   - Tạo GatewayOrderCode: `ORD{orderUid}-{GUID}`
   - Lưu Payment record vào database với PaymentMethod = "SEPAY"
3. **Server trả về** QR Code URL
4. **User quét QR code** và thanh toán qua ứng dụng ngân hàng
5. **Sepay gửi webhook** về server khi thanh toán thành công
6. **Server cập nhật** trạng thái Order và Payment

---

## API Endpoints

### 📋 Tóm tắt nhanh:

- **Endpoint 1 - Tạo QR Code**: Client gọi → Server tạo QR Code URL → Trả về cho Client
- **Endpoint 3 - Webhook/Callback**: Sepay gọi → Server nhận thông báo thanh toán → Tự động cập nhật trạng thái

---

### 1. Tạo QR Code Thanh Toán (Client gọi)

**Endpoint:** `POST /api/payment/create-payment-link`

**Mục đích:** Client gọi endpoint này để tạo QR Code URL cho user quét và thanh toán.

**Authorization:** Bearer Token (Required)

**Request Body:**
```json
{
  "userUid": "550e8400-e29b-41d4-a716-446655440000",
  "planUid": "660e8400-e29b-41d4-a716-446655440001"
}
```

**Lưu ý:** Sepay không hỗ trợ redirect flow như PayOS, nên không cần `returnUrl` và `cancelUrl`.

**Response Success (200 OK):**
```json
{
  "success": true,
  "data": {
    "uid": "880e8400-e29b-41d4-a716-446655440003",
    "checkoutUrl": "https://qr.sepay.vn/img?acc=1234567890&bank=VCB&amount=299000&des=CYBERCITY-ORD550e8400-12345678",
    "qrCode": "https://qr.sepay.vn/img?acc=1234567890&bank=VCB&amount=299000&des=CYBERCITY-ORD550e8400-12345678",
    "orderCode": 12345678,
    "status": "pending",
    "amount": 299000,
    "description": "Nguyễn_Văn_A_Gói_Premium_30days",
    "userName": "Nguyễn Văn A",
    "planName": "Gói Premium"
  },
  "message": "Tạo link thanh toán thành công"
}
```

**Lưu ý:**
- `checkoutUrl` và `qrCode` là cùng một QR Code URL từ Sepay
- `orderCode` là phần cuối của GatewayOrderCode (sau dấu `-`)
- `description` trong response: `{userName}_{planName}_{durationDays}days`
- `description` trong QR URL: `CYBERCITY-{GatewayOrderCode}` (dùng để track payment)

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
- `orderCode` (path parameter): Mã đơn hàng (phần cuối của GatewayOrderCode, long integer)

**Response Success (200 OK):**
```json
{
  "success": true,
  "data": {
    "orderCode": 12345678,
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

### 3. Webhook/Callback từ Sepay (Sepay gọi về server)

**Endpoint:** `POST /api/payment/webhook` hoặc `POST /api/payment/webhook/sepay`

**Mục đích:** Sepay tự động gọi endpoint này khi thanh toán thành công để thông báo cho server.

**Authorization:** Header `Apikey {token}` hoặc `Authorization: Apikey {token}` (Public endpoint nhưng yêu cầu verify token)

**⚠️ Lưu ý quan trọng:**
- Endpoint này KHÔNG phải do Client gọi, mà là Sepay tự động gọi về server
- Cần cấu hình webhook URL trong Sepay dashboard để trỏ về endpoint này
- Server sẽ tự động cập nhật trạng thái payment và order khi nhận được webhook

**Request Headers:**
```
Apikey: your-webhook-token-here
Content-Type: application/json
```

**Request Body:** (Tự động gửi từ Sepay khi thanh toán thành công)
```json
{
  "id": 123456,
  "amount": 299000,
  "transferAmount": 299000,
  "description": "CYBERCITY-ORD550e8400-12345678",
  "content": "CYBERCITY-ORD550e8400-12345678",
  "transaction_code": "TXN123456789",
  "transId": "TXN123456789",
  "referenceCode": "TXN123456789"
}
```

**Lưu ý:**
- Sepay có thể gửi các trường khác nhau trong payload
- Server sẽ tìm kiếm: `amount` hoặc `transferAmount`, `description` hoặc `content`, `transaction_code` hoặc `transId` hoặc `referenceCode`
- Server sẽ tìm payment dựa trên `description` chứa format: `CYBERCITY-ORD{orderUid}-{guid}`
- GatewayOrderCode format: `ORD{orderUid}-{guid}` (8 ký tự cuối là orderCode)

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
- `orderCode` (path parameter): Mã đơn hàng (phần cuối của GatewayOrderCode)
- `reason` (query parameter – optional): Lý do hủy

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
- payment_method: string (SEPAY)
- transaction_code: string (GatewayOrderCode: ORD{orderUid}-{GUID})
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
}

const createPayment = async (userId: string, planId: string) => {
  const request: CreatePaymentRequest = {
    userUid: userId,
    planUid: planId
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
      // Hiển thị QR code để user quét
      // QR URL từ Sepay: result.data.qrCode
      setQrCodeUrl(result.data.qrCode);
      
      // Hoặc redirect đến trang hiển thị QR code
      // window.location.href = `/payment/qr?url=${encodeURIComponent(result.data.qrCode)}`;
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
POST https://localhost:7168/api/payment/create-payment-link
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "userUid": "YOUR_USER_UID",
  "planUid": "YOUR_PLAN_UID"
}
```

### 2. Kiểm tra Status

```bash
GET https://localhost:7168/api/payment/status/12345678
Authorization: Bearer YOUR_JWT_TOKEN
```

### 3. Test Webhook (Sepay)

**⚠️ Lưu ý:** Endpoint này thường được Sepay tự động gọi, nhưng bạn có thể test thủ công bằng Postman:

```bash
POST https://localhost:7168/api/payment/webhook/sepay
Apikey: YOUR_WEBHOOK_TOKEN
Content-Type: application/json

{
  "id": 123456,
  "amount": 299000,
  "description": "CYBERCITY-ORD550e8400-12345678",
  "transaction_code": "TXN123456789"
}
```

**Cấu hình Webhook trong Sepay Dashboard:**
- Webhook URL: `https://your-domain.com/api/payment/webhook/sepay`
- Webhook Token: Giá trị từ `appsettings.json` → `Sepay:WebhookToken`

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
- Thiếu cấu hình Sepay → `Thiếu cấu hình Sepay:BankCode hoặc Sepay:AccountNumber`
- Sepay error → `Failed to create payment link: [error message]`

### 🔒 Security:

- Tất cả endpoints (trừ webhook) yêu cầu JWT token
- Amount được lấy từ database, không cho phép client tự set
- Webhook cần verify Apikey header từ Sepay (config: `Sepay:WebhookToken`)

---

## Sepay Configuration

Trong `appsettings.json`:

```json
{
  "Sepay": {
    "BankCode": "VCB",
    "AccountNumber": "1234567890",
    "WebhookToken": "your-webhook-token-here"
  }
}
```

**Giải thích các trường:**
- `BankCode`: Mã ngân hàng (VD: VCB, TCB, ACB, etc.) - dùng để tạo QR Code
- `AccountNumber`: Số tài khoản ngân hàng nhận tiền - dùng để tạo QR Code
- `WebhookToken`: Token để verify webhook từ Sepay (gửi trong header `Apikey`) - dùng để xác thực callback từ Sepay

**Cách hoạt động:**
1. **Tạo QR Code**: Server sử dụng `BankCode` và `AccountNumber` để tạo QR Code URL
2. **Nhận Callback**: Server sử dụng `WebhookToken` để verify request từ Sepay khi thanh toán thành công

**Format QR Code URL:**
```
https://qr.sepay.vn/img?acc={accountNumber}&bank={bankCode}&amount={amount}&des={description}
```

**Format GatewayOrderCode:**
```
ORD{orderUid}-{GUID}
```
- `orderUid`: 8 ký tự đầu của Order UID
- `GUID`: 8 ký tự từ GUID mới

**Format AddInfo trong QR:**
```
CYBERCITY-{GatewayOrderCode}
```

## Support

- Sepay QR Code Generator: https://qr.sepay.vn/
- Sepay Documentation: Liên hệ Sepay để được hỗ trợ
