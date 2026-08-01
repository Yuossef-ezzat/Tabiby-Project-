using BLL.Dtos.Payment.paymob.Webhook;
using BLL.Services.AbstractServices.PaymobModule.BLL.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{

    [AllowAnonymous] 
    public class PaymobWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymobWebhookController> _logger;

        public PaymobWebhookController(
            IPaymentService paymentService,
            ILogger<PaymobWebhookController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] PaymobWebhookPayload payload)
        {
            var result = await _paymentService.ProcessWebhookAsync(payload);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Paymob webhook processing failed: {Error}", result.Error);
            }

            return Ok();
        }
    }
}
