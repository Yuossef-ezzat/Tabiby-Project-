using BLL.Dtos.Payment;
using BLL.Services.AbstractServices.PaymobModule.BLL.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PresentationLayer.Controller;

namespace PL.Controllers
{

    [Authorize] 
    public class PaymentController : ApiControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        [EnableRateLimiting("Payment")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto request)
        {
            var result = await _paymentService.CreatePaymentAsync(request);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

    }
}
