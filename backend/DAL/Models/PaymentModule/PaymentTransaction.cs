using DAL.Shared;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.PaymentModule
{
    public class PaymentTransaction : BaseEntity
    {
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }

        public string PaymobTransactionId { get; set; } = null!; //Idempotency

        public PaymentTransactionType Type { get; set; }
        public PaymentTransactionStatus Status { get; set; }

        public decimal Amount { get; set; }

        public bool IsVoidable { get; set; }
        public bool IsRefundable { get; set; }
        public bool ErrorOccurred { get; set; }
        public string? RawResponseJson { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
