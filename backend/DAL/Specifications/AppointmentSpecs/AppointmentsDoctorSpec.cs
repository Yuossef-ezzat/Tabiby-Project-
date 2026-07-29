using DAL.Models.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentEntity = DAL.Models.AppointmentModule.Appointment; 

namespace DAL.Specifications.Appointment
{
    public class AppointmentsDoctorSpec : BaseSpecification<AppointmentEntity>
    {
        public AppointmentsDoctorSpec(int doctorId)
        : base(a => a.DoctorId == doctorId)
        {
            AddInclude(a => a.Patient!);
            AddInclude(a => a.Doctor!);
            AddInclude(a => a.Schedule);
            ApplyOrderByDescending(a => a.CreatedAt);
        }
    }
}
