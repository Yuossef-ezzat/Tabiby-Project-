using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errors
{
    public static class AppointmentError
    {
        public static Errror AppointmentNotFound (int appointmentId)
            => new Errror("AppointmentNotFound", $"Appointment '{appointmentId}' was not found.");
        public static Errror Unauthorized(int userId, int appointmentId) 
            => new( "Appointment.Unauthorized",  $"User '{userId}' is not authorized to access appointment '{appointmentId}'.");
        public static Errror InvalidStatus(AppointmentStatus status)
            => new( "Appointment.InvalidStatus", $"Appointment cannot be modified because its status is '{status}'.");

        public static Errror PastAppointment() 
            => new("Appointment.PastAppointment", "Past appointments cannot be modified.");
        public static Errror PatientRoleRequired()
            => new("Appointment.PatientRoleRequired","Only patients can book appointments.");
        public static Errror InvalidDayOfWeek(DayOfWeek expectedDay)
            => new("Appointment.InvalidDayOfWeek", $"The appointment date must be on {expectedDay}.");
    }
}
