using DAL.Models.NursingModule;

namespace DAL.Specifications.NursingRequestSpecs
{
    public class NursingRequestByIdSpec : BaseSpecification<NursingRequest>
    {
        public NursingRequestByIdSpec(int requestId)
            : base(r => r.Id == requestId)
        {
            AddInclude(r => r.Review);
            Includes.Add(r => r.Patient);
            Includes.Add(r => r.Nurse);
        }
    }
}