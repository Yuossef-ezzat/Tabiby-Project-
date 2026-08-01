using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment.paymob
{
    public class RefundRequest
    {
        public string TransactionId { get; set; } = null!;
        public int AmountCents { get; set; }
    }
}
