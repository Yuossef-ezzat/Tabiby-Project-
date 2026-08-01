using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment.paymob.Webhook
{
    public class PaymobWebhookObj
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("amount_cents")]
        public long AmountCents { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("is_capture")]
        public bool IsCapture { get; set; }

        [JsonPropertyName("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonPropertyName("is_auth")]
        public bool IsAuth { get; set; }

        [JsonPropertyName("is_3d_secure")]
        public bool Is3DSecure { get; set; }

        [JsonPropertyName("integration_id")]
        public long IntegrationId { get; set; }

        [JsonPropertyName("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonPropertyName("owner")]
        public long Owner { get; set; }

        [JsonPropertyName("error_occured")]  // deliberately kept misspelled — matches Paymob's field
        public bool ErrorOccured { get; set; }

        [JsonPropertyName("order")]
        public PaymobWebhookOrder Order { get; set; } = null!;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = null!;

        [JsonPropertyName("source_data")]
        public PaymobSourceData? SourceData { get; set; }
    }
    public class PaymobWebhookOrder
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public class PaymobSourceData
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("sub_type")]
        public string? SubType { get; set; }
    }

}
