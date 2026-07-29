using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentEntity = DAL.Models.AppointmentModule.Appointment;

namespace DAL.Specifications.Appointment
{
    public class AppointmentsPatientSpec:BaseSpecification<AppointmentEntity>
    {
        public AppointmentsPatientSpec(int userId)
       : base(a => a.PatientId == userId)
        {
            AddInclude(a => a.Patient!);
            AddInclude(a => a.Doctor!);
            AddInclude(a => a.Schedule);
            ApplyOrderByDescending(a => a.CreatedAt);
        }
    }
}
