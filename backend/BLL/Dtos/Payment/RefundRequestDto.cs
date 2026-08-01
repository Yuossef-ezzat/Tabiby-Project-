using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment
{
    public class RefundRequestDto
    {
        public int PaymentId { get; set; }
        public decimal? Amount { get; set; }   // null = Full refund
    }
}
