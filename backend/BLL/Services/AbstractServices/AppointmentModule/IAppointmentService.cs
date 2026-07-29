using BLL.Abstractions;
using BLL.Dtos.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.AppointmentModule
{
    public interface IAppointmentService
    {
        Task<Result<AppointmentDto>> GetAppointmentAsync(int appointmentId, int userId);
        Task<Result<IEnumerable<AppointmentDto>>> GetMyAppointmentsAsync(int userId);
        Task<Result<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(int doctorId);
        Task<Result<AppointmentDto>> BookAppointmentAsync(int patientId, CreateAppointmentDto dto);
        Task<Result<AppointmentDto>> CancelAppointmentAsync(int appointmentId, int userId);
        Task<Result<AppointmentDto>> ConfirmAppointmentAsync(int appointmentId, int doctorId);
        Task<Result<AppointmentDto>> CompleteAppointmentAsync(int appointmentId, int doctorId);
        Task<Result<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, UpdateAppointmentDto dto, int userId);
        Task<Result<IEnumerable<AvailableDoctorSlotDto>>> GetAvailableSlotsAsync(int doctorId, DateTime date);
    }
}
