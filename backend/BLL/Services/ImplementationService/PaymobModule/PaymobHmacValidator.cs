using BLL.Dtos.Payment.paymob.Webhook;
using BLL.Services.AbstractServices.PaymobModule;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.ImplementationService.PaymobModule
{
    public class PaymobHmacValidator : IPaymobHmacValidator
    {
        private readonly PaymobSettings _paymobSettings;
        public PaymobHmacValidator(IOptions<PaymobSettings> paymobSettings)
        {
            _paymobSettings = paymobSettings.Value;
        }
        public bool IsValid(PaymobWebhookObj obj, string receivedHmac)
        {
            var concatenated = string.Concat(
                obj.AmountCents,
                obj.CreatedAt,
                obj.Currency,
                obj.ErrorOccured,
                obj.HasParentTransaction,
                obj.Id,
                obj.IntegrationId,
                obj.Is3DSecure,
                obj.IsAuth,
                obj.IsCapture,
                obj.IsRefunded,
                obj.IsStandalonePayment,
                obj.IsVoided,
                obj.Order.Id,
                obj.Owner,
                obj.Pending,
                obj.SourceData?.SubType,
                obj.SourceData?.Type,
                obj.Success
            );
            var computedHmac = ComputeHmacSha512(concatenated, _paymobSettings.HmacSecret);

            return string.Equals(computedHmac, receivedHmac, StringComparison.OrdinalIgnoreCase);
        
        }
        private static string ComputeHmacSha512(string data, string secret)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

    }
}
