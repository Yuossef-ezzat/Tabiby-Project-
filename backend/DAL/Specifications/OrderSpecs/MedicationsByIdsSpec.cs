using DAL.Models.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.OrderSpecs
{
    public class MedicationsByIdsSpec : BaseSpecification<Medication>
    {
        public MedicationsByIdsSpec(List<int> ids)
            : base(m => ids.Contains(m.Id))
        {
        }
    }
}
