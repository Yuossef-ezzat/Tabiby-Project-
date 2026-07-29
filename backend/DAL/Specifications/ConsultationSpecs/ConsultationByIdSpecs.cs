using DAL.Models.Consultation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.ConsultationSpecs
{
    public class ConsultationByIdSpecs : BaseSpecification<Consultation>
    {
        public ConsultationByIdSpecs(int consultationId, int requesterId) : base(c => c.Id == consultationId && (c.PatientId == requesterId || c.DoctorId == requesterId))
        {
            Includes.Add(c => c.Patient);
            Includes.Add(c => c.Doctor);
        }
    }
}
