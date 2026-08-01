using DAL.Models.PaymentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications
{
    public class PaymentSpecs : BaseSpecification<Payment>
    {
        public PaymentSpecs(int orderId) : base(p => p.OrderId == orderId && p.PaymobIntentionId != null)
        {
            AddInclude(p => p.Transactions);
            AddInclude(p => p.Order);
        }
    }
}
