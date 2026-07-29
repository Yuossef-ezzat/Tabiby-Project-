using DAL.Models.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.OrderSpecs
{
    public class AllMedicationOrderedSpecs: BaseSpecification<Medication>
    {
        public AllMedicationOrderedSpecs(string? searchName):
            base(m=> (string.IsNullOrWhiteSpace(searchName) || m.Name.ToLower().Contains(searchName.ToLower()) ))
        {
            ApplyOrderByDescending(m=> m.Stock > 0);

        }
    }
}
