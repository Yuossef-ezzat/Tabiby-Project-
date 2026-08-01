using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment
{
    public class CreatePaymentRequestDto
    {
        public int OrderId { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
