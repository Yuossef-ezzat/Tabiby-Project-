using DAL.Models.NursingModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.NursingRequestSpecs
{
    public class ReviewByNurseIdSpecs : BaseSpecification<NursingReview>
    {
        public ReviewByNurseIdSpecs(int nurseId) : base(r => r.NursingRequest.NurseId == nurseId)
        {
            AddInclude(r => r.NursingRequest);
        }
   
    }
}
