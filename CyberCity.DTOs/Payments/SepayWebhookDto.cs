using System;
using System.Text.Json.Serialization;

namespace CyberCity.DTOs.Payments
{
    /// <summary>
    /// DTO cho webhook từ Sepay khi có giao dịch chuyển khoản thành công
    /// Format thực tế từ Sepay API (Nov 2025)
    /// </summary>
    public class SepayWebhookDto
    {
        /// <summary>
        /// ID giao dịch từ Sepay (unique)
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// Mã ngân hàng gateway (VD: "MBBank", "VCB", "TCB")
        /// </summary>
        [JsonPropertyName("gateway")]
        public string? Gateway { get; set; }

        /// <summary>
        /// Thời gian giao dịch (format: "2025-11-10 03:16:00")
        /// </summary>
        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; set; }

        /// <summary>
        /// Số tài khoản nhận tiền
        /// </summary>
        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        /// <summary>
        /// Sub account (thường null)
        /// </summary>
        [JsonPropertyName("subAccount")]
        public string? SubAccount { get; set; }

        /// <summary>
        /// Mã code (thường null)
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Nội dung chuyển khoản (chứa mã đơn hàng)
        /// VD: "MB 77000386190312 CYBERCITYORD56603b131470db2f- Ma GD ACSP/ 1H065781"
        /// </summary>
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        /// <summary>
        /// Loại giao dịch: "in" (vào) hoặc "out" (ra)
        /// </summary>
        [JsonPropertyName("transferType")]
        public string? TransferType { get; set; }

        /// <summary>
        /// Mô tả chi tiết giao dịch
        /// VD: "BankAPINotify MB 77000386190312 CYBERCITYORD56603b131470db2f- Ma GD ACSP/ 1H065781"
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Số tiền chuyển
        /// </summary>
        [JsonPropertyName("transferAmount")]
        public decimal TransferAmount { get; set; }

        /// <summary>
        /// Mã tham chiếu giao dịch từ ngân hàng
        /// VD: "FT25314344802812"
        /// </summary>
        [JsonPropertyName("referenceCode")]
        public string? ReferenceCode { get; set; }

        /// <summary>
        /// Số dư tích lũy sau giao dịch
        /// </summary>
        [JsonPropertyName("accumulated")]
        public decimal? Accumulated { get; set; }
    }
}
