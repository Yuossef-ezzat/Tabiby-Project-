using DAL.Models.OrderModule;
using DAL.Specifications;

namespace DAL.Specifications.OrderSpecs
{
    public class AllOrdersSpecs : BaseSpecification<Order>
    {
        public AllOrdersSpecs() : base()
        {
            Includes.Add(o => o.OrderItem);
            Includes.Add(o => o.Patient);
            ApplyOrderByDescending(o => o.OrderDate);
        }
    }
}
