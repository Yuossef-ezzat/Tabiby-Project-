using DAL.Models.NursingModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.NursingRequestSpecs
{
    public class ReviewByRequestIdSpecs : BaseSpecification<NursingReview>
    {
        public ReviewByRequestIdSpecs(int requestId) : base(r => r.NursingRequestId == requestId)
        {
            AddInclude(r => r.NursingRequest);
            
        }
    }
}