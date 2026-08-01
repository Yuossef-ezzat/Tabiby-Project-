using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment.paymob
{
    public class CreateIntentionRequest
    {
        public int Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public List<int> PaymentMethods { get; set; } = new();
        public BillingData BillingData { get; set; } = null!;
    }
}
