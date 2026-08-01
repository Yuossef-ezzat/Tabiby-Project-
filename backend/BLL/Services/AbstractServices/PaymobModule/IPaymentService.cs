using BLL.Abstractions;
using BLL.Dtos.Payment;
using BLL.Dtos.Payment.paymob.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.PaymobModule
{
    namespace BLL.Services.Payment
    {
        public interface IPaymentService
        {
            Task<Result<PaymentResultDto>> CreatePaymentAsync(CreatePaymentRequestDto request);
            Task<Result<WebhookProcessResultDto>> ProcessWebhookAsync(PaymobWebhookPayload payload);

            Task<Result> RefundPaymentAsync(RefundRequestDto request);
        }
    }
}
