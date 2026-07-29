using DAL.Models.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.OrderSpecs
{
    public class OrderByOrderIdAndPatientIdSpecs : BaseSpecification<Order>
    {
        public OrderByOrderIdAndPatientIdSpecs(int orderId, int patientId)
            : base(o => o.Id == orderId && o.PatientId == patientId)
        {
            AddInclude(o => o.OrderItem);
            AddInclude(o => o.Address);
        }
    }
}
