using DAL.Models.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.Appointment
{
    public class GetExistingScheduleSpecs : BaseSpecification<DoctorSchedule>
    {
        public GetExistingScheduleSpecs(int doctorId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
            : base(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek && s.StartTime == startTime && s.EndTime == endTime)
        {
        }
    }
}
