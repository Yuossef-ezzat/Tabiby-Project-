using DAL.Models.Consultation;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.ConsultationSpecs
{
    public class AllowedConsultationSpecs : BaseSpecification<Consultation>
    {
        public AllowedConsultationSpecs(int patientId ,int doctorId ) :
                base(c => c.PatientId == patientId && c.DoctorId == doctorId && c.Status != ConsultationStatus.Completed)
        {
            
        }
    }
}
