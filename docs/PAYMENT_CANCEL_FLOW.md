# Payment Cancel Flow Documentation

## 📋 Tổng quan

Khi user không thanh toán và rời khỏi trang PayOS, hệ thống tự động đánh dấu order là `failed`.

## 🔄 Luồng xử lý

### 1. **User tạo payment link**
```
POST /api/payment/create-payment-link
→ Tạo Order (payment_status = 'pending')
→ Tạo Payment (status = 'pending')
→ Trả về checkout URL
```

### 2. **User cancel trên PayOS**
PayOS redirect về:
```
GET /api/payment/cancel-callback?orderCode={orderCode}
→ Cập nhật Payment.status = 'failed'
→ Cập nhật Order.payment_status = 'failed'
→ Redirect về frontend: /payment/cancelled
```

### 3. **User quay lại sau khi rời trang (không thanh toán)**
PayOS redirect về:
```
GET /api/payment/return-callback?orderCode={orderCode}
→ Kiểm tra trạng thái từ PayOS
→ Nếu status != 'PAID':
   → Cập nhật status = 'failed'
   → Redirect về frontend: /payment/cancelled
```

### 4. **User thanh toán thành công**
PayOS gọi webhook:
```
POST /api/payment/webhook
→ Cập nhật Payment.status = 'completed'
→ Cập nhật Order.payment_status = 'paid'
→ PayOS redirect về: /payment/success
```

## 🎯 Endpoints

### Cancel Callback
```http
GET /api/payment/cancel-callback?orderCode=1699876543210
```

**Response:** Redirect về frontend
```
http://localhost:5173/payment/cancelled?orderCode=1699876543210
```

### Return Callback
```http
GET /api/payment/return-callback?orderCode=1699876543210
```

**Response:** 
- Nếu đã thanh toán: Redirect `/payment/success`
- Nếu chưa thanh toán: Redirect `/payment/cancelled`

## 🛠️ Cấu hình URLs khi tạo payment

### Backend (Khuyến nghị)
```json
{
  "userUid": "user123",
  "planUid": "plan456",
  "cancelUrl": "https://api.yourdomain.com/api/payment/cancel-callback",
  "returnUrl": "https://api.yourdomain.com/api/payment/return-callback"
}
```

### Frontend (Direct redirect - không xử lý cancel)
```json
{
  "userUid": "user123",
  "planUid": "plan456",
  "cancelUrl": "http://localhost:5173/payment/cancelled",
  "returnUrl": "http://localhost:5173/payment/success"
}
```

## 📊 Database Schema Update

Cần thêm status `'failed'` vào constraint:

```sql
ALTER TABLE orders 
DROP CONSTRAINT IF EXISTS orders_payment_status_check;

ALTER TABLE orders 
ADD CONSTRAINT orders_payment_status_check 
CHECK (payment_status IN ('pending','paid','failed'));

-- Tương tự cho bảng payments nếu có constraint
```

## 🔍 Kiểm tra trạng thái

```http
GET /api/payment/status/{orderCode}
Authorization: Bearer {token}
```

**Response:**
```json
{
  "orderCode": 1699876543210,
  "amount": 500000,
  "status": "CANCELLED", // hoặc "PAID", "PENDING"
  "createdAt": "2024-11-05T10:30:00",
  "canceledAt": "2024-11-05T10:35:00"
}
```

## 🎨 Frontend Integration

### React Example
```tsx
// Khi user click "Cancel" button
const handleCancel = async () => {
  try {
    await axios.post(`/api/payment/cancel/${orderCode}`, {
      reason: "User cancelled"
    });
    router.push('/payment/cancelled');
  } catch (error) {
    console.error('Cancel failed:', error);
  }
};

// Callback page
const PaymentCancelledPage = () => {
  const { orderCode } = useParams();
  
  useEffect(() => {
    // Show message: "Thanh toán đã bị hủy"
    // Option to retry payment
  }, []);
  
  return <div>Thanh toán thất bại</div>;
};
```

## ⚠️ Lưu ý

1. **Không dùng Authorize cho callback endpoints** → PayOS không gửi JWT token
2. **Validation orderCode** → Đảm bảo order tồn tại và thuộc về user
3. **Idempotent** → Gọi nhiều lần không tạo duplicate status update
4. **Logging** → Log mọi cancel action để tracking

## 🚀 Production Checklist

- [ ] Update `cancelUrl` và `returnUrl` thành domain production
- [ ] Thêm constraint `'failed'` vào database
- [ ] Test cancel flow trên sandbox PayOS
- [ ] Implement retry payment mechanism
- [ ] Add notification email khi order failed
- [ ] Setup monitoring/alerts cho failed payments
