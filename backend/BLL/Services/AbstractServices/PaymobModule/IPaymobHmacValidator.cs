using BLL.Dtos.Payment.paymob.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.PaymobModule
{
    public interface IPaymobHmacValidator
    {
        bool IsValid(PaymobWebhookObj obj, string receivedHmac);
    }
}
