using BLL.Abstractions;
using BLL.Dtos.Payment.paymob;
using DAL.Models.PaymentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.PaymobModule
{
    public interface IPaymobClient
    {
        Task<Result<CreateIntentionResponse>> CreateIntentionAsync(Payment payment);
        Task<Result<RefundResponse>> RefundAsync(string transactionId, decimal amount);

    }
}
