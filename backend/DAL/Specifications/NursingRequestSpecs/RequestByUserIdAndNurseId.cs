using DAL.Models.NursingModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.NursingRequestSpecs
{
    public class RequestByUserIdAndNurseId : BaseSpecification<NursingRequest>
    {
        public RequestByUserIdAndNurseId(int PatientId , int NurseId) : base(r=>r.PatientId == PatientId && r.NurseId == NurseId)
        {
            
        }
    }
}
