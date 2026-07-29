using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errors;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Appointment;
using BLL.Services.AbstractServices;
using BLL.Services.AbstractServices.AppointmentModule;
using DAL.Models.AppointmentModule;
using DAL.Models.Users;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications.Appointment;

namespace BLL.Services.ImplementationService.AppointmentModule
{
    public class AppointmentService(IUnitOfWork _unitOfWork, IMapper _mapper, IUserRepository _userRepository, INotificationService _notificationService) : IAppointmentService
    {

        #region Public Methods
        
        public async Task<Result<AppointmentDto>> BookAppointmentAsync(int patientId, CreateAppointmentDto dto)
        {
            var patient = await ValidatePatientAsync(patientId);
            if (patient.IsFailure)
                return Result<AppointmentDto>.Failure(patient.Error);

            var result = ValidateFutureDate(dto.AppointmentDate);
            if (result.IsFailure)
                return Result<AppointmentDto>.Failure(result.Error);

            var doctor = await _userRepository.GetDoctorByIdAsync(dto.DoctorId);
            if (doctor == null)
                return Result<AppointmentDto>.Failure(DoctorError.NotFound(dto.DoctorId));

            if (!doctor.IsActive)
                return Result<AppointmentDto>.Failure(DoctorError.InActive(dto.DoctorId));

            var schedule = await ValidateScheduleAsync(dto.ScheduleId, dto.DoctorId);
            if (schedule.IsFailure)
                return Result<AppointmentDto>.Failure(schedule.Error);

            var res = ValidateDayOfWeek(dto.AppointmentDate, schedule.Value.DayOfWeek);
            if (res.IsFailure)
                return Result<AppointmentDto>.Failure(res.Error);

            res = await ValidateSlotAvailabilityAsync(dto.DoctorId, dto.AppointmentDate, dto.ScheduleId);
            if (res.IsFailure)
                return Result<AppointmentDto>.Failure(res.Error);

            var appointmentEntity = _mapper.Map<Appointment>(dto);
            appointmentEntity.PatientId = patientId;
            appointmentEntity.AppointmentTime = schedule.Value.StartTime;
            appointmentEntity.Status = AppointmentStatus.Pending;
            appointmentEntity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Appointment>().AddAsync(appointmentEntity);
            await _unitOfWork.SaveChangesAsync();
            var appointment = await GetAppointmentOrThrowAsync(appointmentEntity.Id);
            if (appointment.IsFailure)
                return Result<AppointmentDto>.Failure(appointment.Error);

            await _notificationService.SendNotificationAsync($"Patient with name {patient.Value.Fullname} has booked an appointment.", NotificationType.AppointmentBookRequest, appointment.Value.DoctorId);
            var appointmentDto = _mapper.Map<AppointmentDto>(appointment.Value);
            return Result<AppointmentDto>.Success(appointmentDto);
        }

        public async Task<Result<AppointmentDto>> CancelAppointmentAsync(int appointmentId, int userId)
        {
            var appointment = await GetAppointmentOrThrowAsync(appointmentId);
            if (appointment.IsFailure)
                return Result<AppointmentDto>.Failure(appointment.Error);

            var res = ValidateOwnership(appointment.Value, userId);
            if(res.IsFailure)
                return Result<AppointmentDto>.Failure(res.Error);

            if (appointment.Value.Status == AppointmentStatus.Cancelled ||
                appointment.Value.Status == AppointmentStatus.Completed)
                return Result<AppointmentDto>.Failure(AppointmentError.InvalidStatus(appointment.Value.Status));

            appointment.Value.Status = AppointmentStatus.Cancelled;
            appointment.Value.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Appointment>().Update(appointment.Value);
            await _unitOfWork.SaveChangesAsync();

            if (appointment.Value.PatientId == userId)
            {
                await _notificationService.SendNotificationAsync
                    ($"Patient {appointment.Value.Patient?.Fullname ?? ""} has canceled an appointment.", NotificationType.AppointmentCanceled, appointment.Value.DoctorId);
            }
            else if (appointment.Value.DoctorId == userId)
            {
                await _notificationService.SendNotificationAsync
                    ($"Doctor {appointment.Value.Doctor?.Fullname ?? ""} has canceled an appointment.", NotificationType.AppointmentCanceled, appointment.Value.PatientId);
            }

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment.Value);
            return Result<AppointmentDto>.Success(appointmentDto);
        }

        public async Task<Result<AppointmentDto>> ConfirmAppointmentAsync(int appointmentId, int doctorId)
        {
            var appointment = await GetAppointmentOrThrowAsync(appointmentId);
            if (appointment.IsFailure)
                return Result<AppointmentDto>.Failure(appointment.Error);
            if (appointment.Value.DoctorId != doctorId)
                return Result<AppointmentDto>.Failure(AppointmentError.Unauthorized(doctorId, appointmentId));

            if (appointment.Value.Status != AppointmentStatus.Pending)
                return Result<AppointmentDto>.Failure(AppointmentError.InvalidStatus(appointment.Value.Status));

            appointment.Value.Status = AppointmentStatus.Confirmed;
            appointment.Value.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Appointment>().Update(appointment.Value);

            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync($"Doctor {appointment.Value.Doctor?.Fullname ?? ""} has confirmed an appointment.", NotificationType.AppointmentConfirmed, appointment.Value.PatientId);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment.Value);
            return Result<AppointmentDto>.Success(appointmentDto);
        }

        public async Task<Result<AppointmentDto>> CompleteAppointmentAsync(int appointmentId, int doctorId)
        {
            var appointment = await GetAppointmentOrThrowAsync(appointmentId);
            if (appointment.IsFailure)
                return Result<AppointmentDto>.Failure(appointment.Error);

            if (appointment.Value.DoctorId != doctorId)
                return Result<AppointmentDto>.Failure(
                        AppointmentError.Unauthorized(doctorId, appointmentId));

            if (appointment.Value.Status != AppointmentStatus.Confirmed)
                return Result<AppointmentDto>.Failure(
                        AppointmentError.InvalidStatus(appointment.Value.Status));

            appointment.Value.Status = AppointmentStatus.Completed;
            appointment.Value.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Appointment>().Update(appointment.Value);

            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendNotificationAsync($"Doctor {appointment.Value.Doctor?.Fullname ?? ""} has completed an appointment.",
                                                            NotificationType.AppointmentCompleted, appointment.Value.PatientId);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment.Value);

            return Result<AppointmentDto>.Success(appointmentDto);
        }

        public async Task<Result<AppointmentDto>> GetAppointmentAsync(int appointmentId, int userId)
        {
            var appointment = await GetAppointmentOrThrowAsync(appointmentId);
            if (appointment.IsFailure)
                return Result<AppointmentDto>.Failure(appointment.Error);

            var result = ValidateOwnership(appointment.Value, userId);
            if (result.IsFailure)
                return Result<AppointmentDto>.Failure(result.Error);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment.Value);

            return Result<AppointmentDto>.Success(appointmentDto);
        }

        public async Task<Result<IEnumerable<AvailableDoctorSlotDto>>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var doctorSchedules = await _unitOfWork.GetRepository<DoctorSchedule>().GetAllAsync(new GetAvailableSlotsSpecs(doctorId, date));

            var bookedAppointments = await _unitOfWork.GetRepository<Appointment>().GetAllAsync(new AppointmentNotCancelledSpec(doctorId, date));

            var bookedScheduleIds = bookedAppointments.Select(a => a.ScheduleId).ToHashSet();

            var freeSchedules = doctorSchedules.Where(s => !bookedScheduleIds.Contains(s.Id));

            var dtos = _mapper.Map<IEnumerable<AvailableDoctorSlotDto>>(freeSchedules);

            foreach (var dto in dtos)
            {
                dto.AvailableDates = new List<DateTime> { date.Date };
            }
            return Result<IEnumerable<AvailableDoctorSlotDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(int doctorId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>().GetAllAsync(new AppointmentsDoctorSpec(doctorId));
            var appointmentDto = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Result<IEnumerable<AppointmentDto>>.Success(appointmentDto);
        }

        public async Task<Result<IEnumerable<AppointmentDto>>> GetMyAppointmentsAsync(int userId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>().GetAllAsync(new AppointmentsPatientSpec(userId));

            var appointmentDto = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return Result<IEnumerable<AppointmentDto>>.Success(appointmentDto);
        }

        public async Task<Result<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, UpdateAppointmentDto dto, int userId)
        {
            var appointment = await GetAppointmentOrThrowAsync(appointmentId);
            if (appointment.IsFailure)
                return Result<AppointmentDto>.Failure(appointment.Error);

            var res = ValidateOwnership(appointment.Value, userId);
            if (res.IsFailure)
                return Result<AppointmentDto>.Failure(res.Error);

            if (appointment.Value.Status == AppointmentStatus.Cancelled)
                return Result<AppointmentDto>.Failure(AppointmentError.InvalidStatus(appointment.Value.Status));

            if (appointment.Value.AppointmentDate < DateTime.Now)
                return Result<AppointmentDto>.Failure(AppointmentError.PastAppointment());
                    
            
            if (dto.ScheduleId.HasValue)
            {
                var schedule = await ValidateScheduleAsync(dto.ScheduleId.Value, appointment.Value.DoctorId);
                if (schedule.IsFailure)
                    return Result<AppointmentDto>.Failure(schedule.Error);
                appointment.Value.AppointmentTime = schedule.Value.StartTime;
            }

            
            if (dto.AppointmentDate.HasValue)
            {
                res = ValidateFutureDate(dto.AppointmentDate.Value);
                if(res.IsFailure)
                    return Result<AppointmentDto>.Failure(res.Error);
                var scheduleId = dto.ScheduleId ?? appointment.Value.ScheduleId;
                var currentSchedule = await _unitOfWork.GetRepository<DoctorSchedule>().GetByIdAsync(scheduleId);
                if (currentSchedule != null)
                {
                    res = ValidateDayOfWeek(dto.AppointmentDate.Value, currentSchedule.DayOfWeek);
                    if (res.IsFailure)
                        return Result<AppointmentDto>.Failure(res.Error);
                }
            }

            
            if (dto.ScheduleId.HasValue || dto.AppointmentDate.HasValue)
            {
                var checkDate = dto.AppointmentDate ?? appointment.Value.AppointmentDate;
                var checkScheduleId = dto.ScheduleId ?? appointment.Value.ScheduleId;
                res = await ValidateSlotAvailabilityAsync(appointment.Value.DoctorId, checkDate, checkScheduleId, appointmentId);
                if (res.IsFailure)
                    return Result<AppointmentDto>.Failure(res.Error);
                if (appointment.Value.Status == AppointmentStatus.Confirmed)
                {
                    appointment.Value.Status = AppointmentStatus.Pending;
                }
            }

            _mapper.Map(dto, appointment.Value);
            appointment.Value.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Appointment>().Update(appointment.Value);
            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendNotificationAsync($"Patient {appointment.Value.Patient!.Fullname ?? ""} has updated an appointment.", NotificationType.AppointmentUpdated, appointment.Value.DoctorId);
            var appointmentDto = _mapper.Map<AppointmentDto>(appointment.Value);
            return Result<AppointmentDto>.Success(appointmentDto);
        }

        #endregion

        #region Private Validation Methods

        private async Task<Result<Appointment>> GetAppointmentOrThrowAsync(int appointmentId)
        {
            var appointment = (await _unitOfWork.GetRepository<Appointment>().GetAllAsync(new AppointmentWithIncludesSpecs(appointmentId))).FirstOrDefault();
            if (appointment == null)
                return Result<Appointment>.Failure(AppointmentError.AppointmentNotFound(appointmentId));
            return Result<Appointment>.Success(appointment);
        }

        private static Result<bool> ValidateOwnership(Appointment appointment, int userId)
        {
            if (appointment.PatientId != userId && appointment.DoctorId != userId)
                return Result<bool>.Failure(AppointmentError.Unauthorized(userId, appointment.Id));
            return Result<bool>.Success(true);
        }

        private async Task<Result<Patient>> ValidatePatientAsync(int patientId)
        {
            var patient = await _userRepository.GetPatientWithAppointmentAsync(patientId);
            if (patient == null)
                return Result<Patient>.Failure(PatientError.PatientNotFound(patientId));
            if (patient.UserType != "Patient")
                return Result<Patient>.Failure(AppointmentError.PatientRoleRequired());

            return Result<Patient>.Success(patient);
        }

        private async Task<Result<DoctorSchedule>> ValidateScheduleAsync(int scheduleId, int doctorId)
        {
            var schedule = await _unitOfWork.GetRepository<DoctorSchedule>().GetByIdAsync(scheduleId);
            if (schedule is null)
                return Result<DoctorSchedule>.Failure(ScheduleError.ScheduleNotFound(scheduleId));
            if (schedule.DoctorId != doctorId)
                return Result<DoctorSchedule>.Failure(AppointmentError.Unauthorized(doctorId, scheduleId));

            if (!schedule.IsAvailable)
                return Result<DoctorSchedule>.Failure(ScheduleError.SlotUnavailable(schedule.StartTime, DateTime.Today, doctorId));

            return Result<DoctorSchedule>.Success(schedule);
        }

        private async Task<Result<bool>> ValidateSlotAvailabilityAsync(int doctorId, DateTime date, int scheduleId, int? excludeAppointmentId = null)
        {

            var bookedAppointments = await _unitOfWork.GetRepository<Appointment>()
                                    .GetAllAsync(new AppointmentNotCancelledSpec(doctorId, date));

            var isSlotTaken = bookedAppointments.Any(a => a.ScheduleId == scheduleId && a.Id != excludeAppointmentId);
           
            if (isSlotTaken)
            {
                var schedule = await _unitOfWork.GetRepository<DoctorSchedule>().GetByIdAsync(scheduleId);
                return Result<bool>.Failure(ScheduleError.SlotUnavailable(schedule?.StartTime ?? TimeSpan.Zero, date, doctorId));
            }
            return Result<bool>.Success(true);
        }


        private static Result<bool> ValidateFutureDate(DateTime date)
        {
            if (date.Date < DateTime.Today)
                return Result<bool>.Failure(AppointmentError.PastAppointment());
            return Result<bool>.Success(true);
        }

        private static Result<bool> ValidateDayOfWeek(DateTime date, DayOfWeek expectedDay)
        {
            if (date.DayOfWeek != expectedDay)
                return Result<bool>.Failure(AppointmentError.InvalidDayOfWeek(expectedDay));
            return Result<bool>.Success(true);
        }

        #endregion
    }
}
