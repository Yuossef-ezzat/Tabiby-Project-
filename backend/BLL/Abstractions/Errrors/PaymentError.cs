using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class PaymentError
    {
        public static Errror InvalidIntegrationId 
            => new Errror("InvalidIntegrationId", "The provided integration ID is invalid.");
        public static Errror FailedToCreateIntention
            => new Errror("FailedToCreateIntention", "Failed to create payment intention.");
        public static Errror InvalidHMAC
            => new Errror("Invalid HMAC — callback rejected.", "The provided HMAC is invalid.");
        public static Errror PaymentNotFound(int orderId)
            => new Errror("PaymentNotFound", $"No matching Payment found for Paymob order {orderId}.");
    }
}
