using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class DoctorScheduleError
    {
        public static Errror DoctorScheduleConflict(DayOfWeek dayOfWeek , TimeSpan startTime, TimeSpan endTime)
            => new ("DoctorScheduleConflict", $"A schedule already exists for {dayOfWeek} from {startTime} to {endTime}.");
        public static Errror DoctorScheduleNotFound(int scheduleId)
            => new ("DoctorScheduleNotFound", $"No schedule found with ID {scheduleId}.");
        public static Errror UnAuthorizedAccess(int doctorId, int scheduleId)
            => new ("UnAuthorizedAccess", $"Doctor with ID {doctorId} is not authorized to access schedule with ID {scheduleId}.");
        
    }
}
