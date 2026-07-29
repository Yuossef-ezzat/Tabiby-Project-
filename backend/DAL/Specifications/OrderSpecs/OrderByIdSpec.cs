using DAL.Models.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.OrderSpecs
{
    public class OrderByIdSpec  : BaseSpecification<Order>
    {
        public OrderByIdSpec(int orderId) : base(o => o.Id == orderId)
        {
            AddInclude(o=> o.OrderItem);
        }
    }
}
