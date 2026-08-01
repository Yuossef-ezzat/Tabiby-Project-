using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Payment;
using BLL.Dtos.Payment.paymob.Webhook;
using BLL.Services.AbstractServices.PaymobModule;
using BLL.Services.AbstractServices.PaymobModule.BLL.Services.Payment;
using DAL.Models.OrderModule;
using DAL.Models.PaymentModule;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System.Text.Json;

namespace BLL.Services.ImplementationService.PaymobModule
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork ;
        private readonly IPaymobClient _paymobClient;
        private readonly PaymobSettings _paymobSettings;
        private readonly IPaymobHmacValidator _hmacValidator;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IPaymobClient paymobClient,
            IOptions<PaymobSettings> paymobSettings,
            IPaymobHmacValidator hmacValidator)
        {
            _unitOfWork = unitOfWork;
            _paymobClient = paymobClient;
            _paymobSettings = paymobSettings.Value;
            _hmacValidator = hmacValidator;
        }
        public async Task<Result<PaymentResultDto>> CreatePaymentAsync(CreatePaymentRequestDto dto)
        {
            var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(dto.OrderId);
            if (order == null)
                return Result<PaymentResultDto>.Failure(OrderError.NotFound(dto.OrderId));
            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.Total,
                PaymentMethod = dto.Method,
                Status = PaymentStatus.Pending,
            };
            string? checkoutUrl = null ;
            if(dto.Method != PaymentMethod.Cash)
            {
                payment.IntegrationId = _paymobSettings.GetIntegrationId(dto.Method);
                var intentionResponse = await _paymobClient.CreateIntentionAsync(payment);
                if (!intentionResponse.IsSuccess)
                    return Result<PaymentResultDto>.Failure(intentionResponse.Error);
                payment.PaymobIntentionId = intentionResponse.Value.Id;
                payment.PaymobClientSecret = intentionResponse.Value.ClientSecret;
                checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_paymobSettings.PublicKey}&clientSecret={payment.PaymobClientSecret}";

            }
            await _unitOfWork.GetRepository<Payment>().AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentResultDto>.Success(new PaymentResultDto
            {
                PaymentId = payment.Id,
                CheckoutUrl = checkoutUrl,
                Status = payment.Status,
            });
        }

        public async Task<Result<WebhookProcessResultDto>> ProcessWebhookAsync(PaymobWebhookPayload payload)
        {
            if (!_hmacValidator.IsValid(payload.Obj, payload.Hmac))
                return Result<WebhookProcessResultDto>.Failure(PaymentError.InvalidHMAC);
            var transactionId = payload.Obj.Id.ToString();
            var existing = (await _unitOfWork.GetRepository<PaymentTransaction>().GetAllAsync(new PaymentTransactionByKey(transactionId))).FirstOrDefault();

            if (existing is not null)
            {
                return Result<WebhookProcessResultDto>.Success(new WebhookProcessResultDto
                {
                    PaymentId = existing.PaymentId,
                    WasAlreadyProcessed = true
                });
            }
            var payment = (await _unitOfWork.GetRepository<Payment>().GetAllAsync(new PaymentSpecs((int)payload.Obj.Order.Id))).FirstOrDefault();
            if (payment is null)
                return Result<WebhookProcessResultDto>.Failure(PaymentError.PaymentNotFound((int)payload.Obj.Order.Id));
            var status = payload.Obj.Success
                ? PaymentTransactionStatus.Success
                : payload.Obj.Pending
                    ? PaymentTransactionStatus.Pending
                    : PaymentTransactionStatus.Failed;
            var type = payload.Obj.IsRefunded
                   ? PaymentTransactionType.Refund
                   : PaymentTransactionType.Charge;

            var transaction = new PaymentTransaction
            {
                PaymentId = payment.Id,
                PaymobTransactionId = transactionId,
                Type = type,
                Status = status,
                Amount = payload.Obj.AmountCents / 100m,
                ErrorOccurred = payload.Obj.ErrorOccured,
                ProcessedAt = DateTime.UtcNow,
                RawResponseJson = JsonSerializer.Serialize(payload)
            };
            await _unitOfWork.GetRepository<PaymentTransaction>().AddAsync(transaction);
            if (status == PaymentTransactionStatus.Success && type == PaymentTransactionType.Charge)
                payment.Status = PaymentStatus.Paid;
            else if (status == PaymentTransactionStatus.Failed && type == PaymentTransactionType.Charge)
                payment.Status = PaymentStatus.Failed;
            else if (type == PaymentTransactionType.Refund)
                payment.Status = PaymentStatus.Refunded;

            // Idempotency guard #2 — the real safety net: DB unique index on PaymobTransactionId
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                // A concurrent retry beat us to it — treat as already processed, not an error
                return Result<WebhookProcessResultDto>.Success(new WebhookProcessResultDto
                {
                    PaymentId = payment.Id,
                    WasAlreadyProcessed = true
                });
            }

            return Result<WebhookProcessResultDto>.Success(new WebhookProcessResultDto
            {
                PaymentId = payment.Id,
                WasAlreadyProcessed = false
            });
        }
        bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                // 2601 = Unique index violation, 2627 = Unique constraint violation
                return sqlEx.Number is 2601 or 2627;
            }
            return false;
        }

        public Task<Result> RefundPaymentAsync(RefundRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
