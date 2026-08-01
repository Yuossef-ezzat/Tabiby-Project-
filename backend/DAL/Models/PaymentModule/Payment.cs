using DAL.Models.OrderModule;
using DAL.Models.Users;
using DAL.Shared;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.PaymentModule
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public int? IntegrationId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        // من Paymob - بترجع بعد ما تعمل Create Intention
        public string? PaymobIntentionId { get; set; }
        public string? PaymobClientSecret { get; set; }

        public ICollection<PaymentTransaction> Transactions { get; set; }= new List<PaymentTransaction>();

    }
}
