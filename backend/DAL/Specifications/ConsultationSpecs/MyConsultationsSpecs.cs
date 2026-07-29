using DAL.Models.Consultation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.ConsultationSpecs
{
    public class MyConsultationsSpecs : BaseSpecification<Consultation>
    {
        public MyConsultationsSpecs(int requesterId) : base(c => c.PatientId == requesterId || c.DoctorId == requesterId)
        {
            Includes.Add(c => c.Patient);
            Includes.Add(c => c.Doctor);
            Includes.Add(c => c.Review);
            ApplyOrderByDescending(c => c.RequestedAt);
        }
    }
}
