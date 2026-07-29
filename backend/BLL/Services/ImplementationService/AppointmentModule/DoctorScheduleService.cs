using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Schedule;
using BLL.Services.AbstractServices.AppointmentModule;
using DAL.Models.AppointmentModule;
using DAL.Repository;
using DAL.Specifications;
using DAL.Specifications.Appointment;

namespace BLL.Services.ImplementationService.AppointmentModule
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenaricRepository<DoctorSchedule> _scheduleRepo;

        public DoctorScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _scheduleRepo = _unitOfWork.GetRepository<DoctorSchedule>();
        }

        public async Task<Result<DoctorScheduleDto>> CreateScheduleAsync(int doctorId, CreateDoctorScheduleDto dto)
        {
            
            var spec = new GetExistingScheduleSpecs(doctorId, dto.DayOfWeek, dto.StartTime, dto.EndTime);
              
            
            var existing = await _scheduleRepo.GetAllAsync(spec);
            if (existing.Any())
                return Result<DoctorScheduleDto>.Failure(DoctorScheduleError.DoctorScheduleConflict(dto.DayOfWeek, dto.StartTime, dto.EndTime));
            var schedule = _mapper.Map<DoctorSchedule>(dto);
            schedule.DoctorId = doctorId;
            
            await _scheduleRepo.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return Result<DoctorScheduleDto>.Success(_mapper.Map<DoctorScheduleDto>(schedule));
        }

        public async Task<Result> DeleteScheduleAsync(int scheduleId, int doctorId)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
            if (schedule == null)
                return Result.Failure(DoctorScheduleError.DoctorScheduleNotFound(scheduleId));
            
            if (schedule.DoctorId != doctorId)
                return Result.Failure(DoctorScheduleError.UnAuthorizedAccess(doctorId, scheduleId));

            _scheduleRepo.Delete(schedule);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }

        public async Task<Result<IEnumerable<DoctorScheduleDto>>> GetDoctorSchedulesAsync(int doctorId)
        {
            var spec = new BaseSpecification<DoctorSchedule>(s => s.DoctorId == doctorId);
            var schedules = await _scheduleRepo.GetAllAsync(spec);
            return Result<IEnumerable<DoctorScheduleDto>>.Success(_mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules));
        }

        public async Task<Result<DoctorScheduleDto>> UpdateScheduleAsync(int scheduleId, UpdateDoctorScheduleDto dto, int doctorId)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
            if(schedule is null)
                return Result<DoctorScheduleDto>.Failure(DoctorScheduleError.DoctorScheduleNotFound(scheduleId));
            if (schedule.DoctorId != doctorId)
                return Result<DoctorScheduleDto>.Failure(DoctorScheduleError.UnAuthorizedAccess(doctorId, scheduleId));
            _mapper.Map(dto, schedule);

            _scheduleRepo.Update(schedule);
            await _unitOfWork.SaveChangesAsync();

            return Result<DoctorScheduleDto>.Success(_mapper.Map<DoctorScheduleDto>(schedule));
        }
    }
}
