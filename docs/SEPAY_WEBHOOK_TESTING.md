# Hướng dẫn Test và Debug Sepay Webhook

> **Ngày cập nhật**: November 10, 2025  
> **Mục đích**: Debug và fix lỗi HTTP 400 khi nhận webhook từ Sepay

---

## 🔍 Vấn đề gặp phải

**Triệu chứng**: 
- ✅ Tạo đơn hàng thành công
- ✅ Quét QR thành công
- ✅ Sepay gửi webhook về backend
- ❌ Backend trả về HTTP 400 Bad Request

**Nguyên nhân**: 
- Model binding failed - JSON keys từ Sepay không match với DTO properties
- Missing `[JsonPropertyName]` attributes để map snake_case sang PascalCase
- Không có JSON options để xử lý case-insensitive

---

## ✅ Giải pháp đã implement

### 1. Tạo DTO chuẩn cho Sepay (`SepayWebhookDto.cs`)

```csharp
public class SepayWebhookDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; }

    [JsonPropertyName("bank_code")]
    public string BankCode { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("transaction_date")]
    public string TransactionDate { get; set; }

    [JsonPropertyName("transaction_code")]
    public string TransactionCode { get; set; }

    [JsonPropertyName("reference_code")]
    public string ReferenceCode { get; set; }

    [JsonPropertyName("trans_id")]
    public string TransId { get; set; }

    [JsonPropertyName("transfer_amount")]
    public decimal? TransferAmount { get; set; }
}
```

**Lưu ý**: Sepay có thể gửi các fields khác nhau tùy theo loại giao dịch, nên DTO hỗ trợ nhiều tên field (description/content, transaction_code/trans_id/reference_code...)

---

### 2. Cấu hình JSON Serialization trong `Program.cs`

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Cho phép case-insensitive (snake_case, camelCase, PascalCase đều OK)
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        
        // Giữ nguyên tên properties (không convert sang camelCase)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        
        // Cho phép trailing commas trong JSON
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        
        // Cho phép đọc số dạng string
        options.JsonSerializerOptions.NumberHandling = 
            System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });
```

**Tại sao cần**: ASP.NET mặc định chỉ match exact case, nhưng Sepay gửi snake_case → cần case-insensitive.

---

### 3. Update Controller để dùng Model Binding

**Trước** (đọc raw JSON):
```csharp
[HttpPost("webhook")]
public async Task<IActionResult> SepayWebhook()
{
    using var reader = new StreamReader(Request.Body, Encoding.UTF8);
    var payloadJson = await reader.ReadToEndAsync();
    // Manual parsing...
}
```

**Sau** (model binding):
```csharp
[HttpPost("webhook")]
[AllowAnonymous]
public async Task<IActionResult> SepayWebhook([FromBody] SepayWebhookDto webhookData)
{
    // ASP.NET tự động parse JSON → DTO
    if (webhookData == null)
    {
        return BadRequest(new
        {
            success = false,
            message = "Invalid webhook payload - model binding failed",
            error = "NULL_DATA"
        });
    }
    
    // Lấy token từ nhiều headers
    var token = Request.Headers["Authorization"].ToString();
    if (string.IsNullOrEmpty(token))
        token = Request.Headers["Apikey"].ToString();
    if (string.IsNullOrEmpty(token))
        token = Request.Headers["token"].ToString();
    
    var result = await _paymentService.ProcessSepayWebhookAsync(token, webhookData);
    
    return result 
        ? Ok(new { success = true, message = "Webhook processed successfully" })
        : BadRequest(new { success = false, message = "Payment not found" });
}
```

**Lợi ích**:
- ASP.NET tự động validate và parse JSON
- Trả về 400 với error message rõ ràng nếu JSON không match
- Code sạch hơn, dễ maintain

---

### 4. Update Service Layer

```csharp
public async Task<bool> ProcessSepayWebhookAsync(string authorizationHeader, SepayWebhookDto webhookData)
{
    // Verify token
    var expectedToken = _configuration["Sepay:WebhookToken"];
    if (!string.IsNullOrEmpty(expectedToken))
    {
        var expectedHeader = $"Apikey {expectedToken}";
        if (!string.Equals(authorizationHeader, expectedHeader, StringComparison.Ordinal))
        {
            _logger.LogWarning("Invalid webhook token");
            return false;
        }
    }
    
    // Extract data
    var description = webhookData.Description ?? webhookData.Content;
    var amount = webhookData.Amount > 0 ? webhookData.Amount : webhookData.TransferAmount ?? 0m;
    var transactionRef = webhookData.TransactionCode ?? webhookData.TransId ?? webhookData.ReferenceCode;
    
    // Tìm payment theo description pattern: CYBERCITY-ORD{uid}-{guid}
    // ... existing logic ...
}
```

---

## 🧪 Cách test

### 1. Test local với Postman/curl

**Endpoint**: `POST http://localhost:7168/api/payment/webhook`

**Headers**:
```
Content-Type: application/json
Apikey: nguyenquochuy1098710987
```
(hoặc `Authorization: Apikey nguyenquochuy1098710987`)

**Body (JSON)**:
```json
{
  "id": 29826835,
  "account_number": "77000386190312",
  "bank_code": "MB",
  "amount": 299000,
  "description": "CYBERCITY-ORD12345678-abcd1234",
  "transaction_date": "2025-11-10T03:16:17Z",
  "transaction_code": "FT123456789"
}
```

**Expected Response** (200 OK):
```json
{
  "success": true,
  "message": "Webhook processed successfully"
}
```

**Nếu lỗi 400**:
```json
{
  "success": false,
  "message": "Invalid webhook payload - model binding failed",
  "error": "NULL_DATA",
  "hint": "Check if JSON keys match the DTO properties (case-sensitive)"
}
```

---

### 2. Test với Sepay thật

1. **Tạo payment link mới**:
```bash
POST /api/payment/create-payment-link
{
  "userUid": "U001",
  "planUid": "PLAN001"
}
```

2. **Quét QR và chuyển khoản** với nội dung đúng format: `CYBERCITY-ORD{...}-{...}`

3. **Kiểm tra logs** trong console:
```
[Webhook] Headers: Authorization: Apikey xxx, Content-Type: application/json, ...
[Webhook] Received - Id: 29826835, Amount: 299000, Description: CYBERCITY-ORD...
[ProcessSepayWebhook] Token verification passed
[ProcessSepayWebhook] Found payment by gatewayOrderCode: PAY-xxx
[ProcessSepayWebhook] Payment PAY-xxx processed successfully
[Webhook] SUCCESS
```

4. **Kiểm tra trong Sepay Dashboard**:
   - Vào "Webhooks" → Lịch sử giao dịch #29826835
   - HTTP Status Code phải là **200** (không phải 400)
   - Response body: `{"success":true,"message":"Webhook processed successfully"}`

---

### 3. Debug nếu vẫn lỗi 400

#### Bước 1: Check logs trong console

**Nếu thấy**:
```
[Webhook] ERROR: Webhook data is null (model binding failed)
```
→ **Nguyên nhân**: JSON keys không match với DTO

**Giải pháp**:
- Xem chính xác JSON mà Sepay gửi (copy từ Sepay Dashboard → Chi tiết webhook)
- So sánh với properties trong `SepayWebhookDto.cs`
- Thêm `[JsonPropertyName("key_from_sepay")]` nếu thiếu

---

#### Bước 2: Test với raw JSON endpoint (backup)

Tạm thời thêm endpoint test để log raw JSON:

```csharp
[HttpPost("webhook/debug")]
[AllowAnonymous]
public async Task<IActionResult> SepayWebhookDebug()
{
    using var reader = new StreamReader(Request.Body, Encoding.UTF8);
    var rawJson = await reader.ReadToEndAsync();
    
    Console.WriteLine($"[DEBUG] Raw JSON received:");
    Console.WriteLine(rawJson);
    
    Console.WriteLine($"[DEBUG] Headers:");
    foreach (var header in Request.Headers)
    {
        Console.WriteLine($"  {header.Key}: {header.Value}");
    }
    
    return Ok(new { success = true, message = "Debug logged" });
}
```

Sau đó cấu hình Sepay gửi webhook đến `/api/payment/webhook/debug` để xem raw JSON.

---

#### Bước 3: Kiểm tra appsettings.json

```json
{
  "Sepay": {
    "BankCode": "MB",
    "AccountNumber": "77000386190312",
    "WebhookToken": "nguyenquochuy1098710987"
  }
}
```

**Lưu ý**: `WebhookToken` phải khớp với token bạn cấu hình trong Sepay Dashboard.

---

## 📋 Checklist deploy

- [ ] Build thành công: `dotnet build`
- [ ] Test local với Postman trước → phải trả về 200
- [ ] Deploy lên server (Render/Azure/AWS...)
- [ ] Update webhook URL trong Sepay Dashboard → `https://your-domain.com/api/payment/webhook`
- [ ] Test bằng cách "Bắn lại" webhook cũ trong Sepay Dashboard
- [ ] Tạo đơn mới và chuyển khoản thật để test end-to-end
- [ ] Kiểm tra logs trên server
- [ ] Verify payment status: `GET /api/payment/status/{orderCode}`

---

## 🎯 Kết quả mong đợi

✅ **Trước fix**: HTTP 400 Bad Request  
✅ **Sau fix**: HTTP 200 OK + `{"success":true}`

✅ **Database updated**:
- `Payment.Status` = `"completed"`
- `Payment.PaidAt` = thời gian hiện tại
- `Order.PaymentStatus` = `"paid"`

✅ **User có thể**:
- Xem invoice: `GET /api/payment/invoice/{paymentUid}`
- Kiểm tra subscription active

---

## 🆘 Liên hệ support

Nếu vẫn gặp lỗi sau khi làm theo guide này:

1. Copy **toàn bộ logs** từ console
2. Copy **raw JSON** từ Sepay Dashboard (Chi tiết webhook → Request body)
3. Copy **response body** từ Sepay Dashboard
4. Gửi cho team để debug

---

**Happy coding! 🚀**
