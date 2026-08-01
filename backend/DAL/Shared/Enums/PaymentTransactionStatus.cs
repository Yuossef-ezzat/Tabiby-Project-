using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Shared.Enums
{
    public enum PaymentTransactionStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2,
        Declined = 3
    }
}
