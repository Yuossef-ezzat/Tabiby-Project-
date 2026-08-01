using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment
{
    public class PaymentResultDto
    {
        public int PaymentId { get; set; }
        public PaymentStatus Status { get; set; }
        public string? CheckoutUrl { get; set; }   // null لو Cash
    }
}
