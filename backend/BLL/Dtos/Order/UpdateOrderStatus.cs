using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Order
{
    public class UpdateOrderStatus
    {
        public OrderStatus Status { get; set; }

    }
}
