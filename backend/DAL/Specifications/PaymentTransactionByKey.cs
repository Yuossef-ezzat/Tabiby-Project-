using DAL.Models.PaymentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications
{
    public class PaymentTransactionByKey : BaseSpecification<PaymentTransaction>
    {
        public PaymentTransactionByKey(string key) : base(p => p.PaymobTransactionId == key)
        {
        }
    }
}
