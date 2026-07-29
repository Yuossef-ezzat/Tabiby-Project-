using BLL.Abstractions;
using BLL.Dtos.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.AppointmentModule
{
    public interface IDoctorScheduleService
    {
        Task<Result<DoctorScheduleDto>> CreateScheduleAsync(int doctorId, CreateDoctorScheduleDto dto);
        Task<Result<IEnumerable<DoctorScheduleDto>>> GetDoctorSchedulesAsync(int doctorId);
        Task<Result<DoctorScheduleDto>> UpdateScheduleAsync(int scheduleId, UpdateDoctorScheduleDto dto, int doctorId);
        Task<Result> DeleteScheduleAsync(int scheduleId, int doctorId);
    }
}
