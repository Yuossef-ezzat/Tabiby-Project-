using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PaymobSettings
    {
        public string BaseUrl { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string HmacSecret { get; set; } = null!;

        public Dictionary<string, int> Integrations { get; set; } = new();

        public int GetIntegrationId(PaymentMethod method)
        {
            if (Integrations.TryGetValue(method.ToString(), out var id))
                return id;

            throw new InvalidOperationException(
                $"No Paymob Integration ID configured for payment method: {method}");
        }
    }
}
