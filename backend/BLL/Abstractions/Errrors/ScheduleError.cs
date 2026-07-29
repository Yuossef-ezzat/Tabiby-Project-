using BLL.Abstractions.Errors;
using DAL.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class ScheduleError
    {
        public static Errror ScheduleNotFound(int scheduleId)
            => new Errror("ScheduleNotFound", $"Schedule '{scheduleId}' was not found.");
        public static Errror SlotUnavailable(TimeSpan startTime, DateTime date,int doctorId) =>
        new(
            "Schedule.SlotUnavailable",
            $"The appointment slot at {startTime:hh\\:mm} on {date:yyyy-MM-dd} with doctor '{doctorId}' is not available.");
    }
}
