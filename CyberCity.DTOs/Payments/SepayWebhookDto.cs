using System;
using System.Text.Json.Serialization;

namespace CyberCity.DTOs.Payments
{
    /// <summary>
    /// DTO cho webhook từ Sepay khi có giao dịch chuyển khoản thành công
    /// </summary>
    public class SepayWebhookDto
    {
        /// <summary>
        /// ID giao dịch từ Sepay
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// Số tài khoản nhận tiền
        /// </summary>
        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Mã ngân hàng (VD: MB, VCB, TCB...)
        /// </summary>
        [JsonPropertyName("bank_code")]
        public string BankCode { get; set; }

        /// <summary>
        /// Số tiền giao dịch
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Nội dung chuyển khoản (chứa mã đơn hàng)
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Nội dung chuyển khoản (tên khác của description)
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// Thời gian giao dịch
        /// </summary>
        [JsonPropertyName("transaction_date")]
        public string TransactionDate { get; set; }

        /// <summary>
        /// Mã giao dịch tham chiếu
        /// </summary>
        [JsonPropertyName("transaction_code")]
        public string TransactionCode { get; set; }

        /// <summary>
        /// Mã giao dịch tham chiếu (tên khác)
        /// </summary>
        [JsonPropertyName("reference_code")]
        public string ReferenceCode { get; set; }

        /// <summary>
        /// ID giao dịch (tên khác)
        /// </summary>
        [JsonPropertyName("trans_id")]
        public string TransId { get; set; }

        /// <summary>
        /// Số tiền chuyển (có thể khác amount do phí)
        /// </summary>
        [JsonPropertyName("transfer_amount")]
        public decimal? TransferAmount { get; set; }

        /// <summary>
        /// Tên tài khoản chuyển tiền
        /// </summary>
        [JsonPropertyName("account_name")]
        public string AccountName { get; set; }

        /// <summary>
        /// Số tài khoản chuyển tiền
        /// </summary>
        [JsonPropertyName("sender_account_number")]
        public string SenderAccountNumber { get; set; }

        /// <summary>
        /// Tên ngân hàng chuyển
        /// </summary>
        [JsonPropertyName("sender_bank_name")]
        public string SenderBankName { get; set; }
    }
}
