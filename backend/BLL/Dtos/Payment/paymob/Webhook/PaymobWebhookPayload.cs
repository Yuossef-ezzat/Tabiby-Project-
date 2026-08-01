using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment.paymob.Webhook
{
    public class PaymobWebhookPayload
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        [JsonPropertyName("obj")]
        public PaymobWebhookObj Obj { get; set; } = null!;

        [JsonPropertyName("hmac")]
        public string Hmac { get; set; } = null!;
    }
}
