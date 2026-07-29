using DAL.Models.Users;
using DAL.Shared;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.OrderModule
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public decimal Total { get => SubTotal + ShippingPrice; }
        public decimal SubTotal { get; set; }
        public decimal ShippingPrice { get; set; } = 20;

                public OrderAddress Address { get; set; } = null!;

        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual List<OrderItem> OrderItem { get; set; }

    }
}
