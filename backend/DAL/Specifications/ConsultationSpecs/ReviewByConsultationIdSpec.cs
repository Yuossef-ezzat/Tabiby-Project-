using DAL.Models.Consultation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.ConsultationSpecs
{
    public class ReviewByConsultationIdSpec : BaseSpecification<ConsultationReview>
    {
        public ReviewByConsultationIdSpec(int ConsultationId) :base(r => r.ConsultationId == ConsultationId)
        {
            
        }
    }
}
