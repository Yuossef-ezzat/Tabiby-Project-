
using AppointmentEntity = DAL.Models.AppointmentModule.Appointment;

namespace DAL.Specifications.Appointment
{
    public class AppointmentWithIncludesSpecs : BaseSpecification<AppointmentEntity>
    {
        public AppointmentWithIncludesSpecs(int appointmentId)
            : base(a => a.Id == appointmentId)
        {
            AddInclude(a => a.Patient!);
            AddInclude(a => a.Doctor!);
            AddInclude(a => a.Schedule);
        }
    }
}
