using DomainLayer.Models.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.OrderSpecs
{
    public class BasketByUserIdSpecs : BaseSpecification<CustomerBasket>
    {
        public BasketByUserIdSpecs(int userId) : base(b => b.PatientId == userId)
        {
            AddInclude(b => b.BasketItems);
        }
    }
}
