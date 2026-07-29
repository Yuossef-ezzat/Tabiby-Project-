using DAL.Models.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.Appointment
{
    public class GetAvailableSlotsSpecs : BaseSpecification<DoctorSchedule>
    {
        public GetAvailableSlotsSpecs(int doctorId, DateTime date)
       : base(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek && s.IsAvailable == true)
        {
        }
    }
}
