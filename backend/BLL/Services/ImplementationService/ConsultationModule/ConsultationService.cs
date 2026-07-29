using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Consultion;
using BLL.Dtos.Doctor;
using BLL.Services.AbstractServices;
using BLL.Services.AbstractServices.ConsultationModule;
using DAL.Models.Consultation;
using DAL.Models.Users;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications.ConsultationSpecs;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services.ImplementationService.ConsultationModule
{
    public class ConsultationService
        (IUnitOfWork _unitOfWork , IMapper _mapper , INotificationService _notificationService 
        , IUserRepository _userRepository , UserManager<ApplicationUser> _userManager) : IConsultationService
    {
        public async Task<Result<DoctorInfoDto>> GetDoctorInfoAsync(int doctorId)
        {
            var doctor = await _userRepository.GetDoctorByIdAsync(doctorId);
            if(doctor == null) 
                return Result<DoctorInfoDto>.Failure(DoctorError.NotFound(doctorId));
            var DoctorDto = _mapper.Map<DoctorInfoDto>(doctor);
            return Result<DoctorInfoDto>.Success(DoctorDto);
        }

        public async Task<Result<IEnumerable<DoctorInfoDto>>> SearchDoctorsAsync(SearchDoctorDto searchDto)
        {
            var doctors = await _userRepository.SearchDoctorsAsync(
                searchDto.Name,
                searchDto.Specialization,
                searchDto.Location,
                searchDto.PageNumber,
                searchDto.PageSize);
            var DoctorsDto = _mapper.Map<IEnumerable<DoctorInfoDto>>(doctors);
            return Result<IEnumerable<DoctorInfoDto>>.Success(DoctorsDto);
        }
        public async Task<Result<ConsultationDto>> RequestConsultationAsync(int PatientId, CreateConsultationDto createDto)
        {
            var existingConsultation = (await _unitOfWork.GetRepository<Consultation>()
                                            .GetAllAsync(new AllowedConsultationSpecs(PatientId, createDto.DoctorId))).FirstOrDefault();
            if (existingConsultation is not null)
                return Result<ConsultationDto>.Failure(ConsultationError.ConsultationAlreadyExists(createDto.DoctorId));

            var patient = await _userManager.FindByIdAsync(PatientId.ToString())
                             as Patient;
            if (patient == null)
                return Result<ConsultationDto>.Failure(PatientError.PatientNotFound(PatientId));

            var doctor = await _userRepository.GetDoctorByIdAsync(createDto.DoctorId);
            if (doctor is null)
                return Result<ConsultationDto>.Failure(DoctorError.NotFound(createDto.DoctorId));

            if (!doctor.IsActive)
                return Result<ConsultationDto>.Failure(DoctorError.DoctorInActive(createDto.DoctorId));

            var consultation = new Consultation
            {
                PatientId = PatientId,
                DoctorId = createDto.DoctorId,
                Status = ConsultationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.GetRepository<Consultation>().AddAsync(consultation);
            await _unitOfWork.SaveChangesAsync();

            

            await _notificationService.SendNotificationAsync( $"You have a new consultation request from {patient.Fullname}.", NotificationType.ConsultationRequest, createDto.DoctorId);
            var consultationDto = _mapper.Map<ConsultationDto>(consultation);
            return Result<ConsultationDto>.Success(consultationDto);
        }

        public async Task<Result> DeleteConsultationAsync(int ConsultationId, int RequesterId)
        {
            var Consultation = (await _unitOfWork.GetRepository<Consultation>()
                                .GetAllAsync(new ConsultationByIdSpecs(ConsultationId, RequesterId))).FirstOrDefault();
            if (Consultation == null)
                return Result.Failure(ConsultationError.ConsultationNotFound(ConsultationId));

            _unitOfWork.GetRepository<Consultation>().Delete(Consultation);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<ConsultationDto>> GetConsultationByIdAsync(int ConsultationId, int RequesterId)
        {
            var consultation = (await _unitOfWork.GetRepository<Consultation>()
                                          .GetAllAsync(new ConsultationByIdSpecs(ConsultationId, RequesterId))).FirstOrDefault();
            if (consultation == null)
                return Result<ConsultationDto>.Failure(ConsultationError.ConsultationNotFound(ConsultationId));

            return Result<ConsultationDto>.Success(_mapper.Map<ConsultationDto>(consultation));
        }

        public async Task<Result<IEnumerable<ConsultationDto>>> GetMyConsultationsAsync(int RequesterId)
        {
            var consultations = await _unitOfWork.GetRepository<Consultation>()
                                        .GetAllAsync(new MyConsultationsSpecs(RequesterId));

            if (!consultations.Any())
                return Result<IEnumerable<ConsultationDto>>.Success(Enumerable.Empty<ConsultationDto>());

            return Result<IEnumerable<ConsultationDto>>.Success(_mapper.Map<IEnumerable<ConsultationDto>>(consultations));
        }

        public async Task<Result<ConsultationDto>> UpdateConsultationStatusAsync(int consultationId, int DoctorId, UpdateConsultionStatusDto updateStatusDto)
        {
            var consultation = await _unitOfWork.GetRepository<Consultation>().GetByIdAsync(consultationId);
            if (consultation == null) 
                return Result<ConsultationDto>.Failure(ConsultationError.ConsultationNotFound(consultationId));

            if (consultation.DoctorId != DoctorId)
                return Result<ConsultationDto>.Failure(ConsultationError.UnauthorizedAccess());

            if (!Enum.TryParse(updateStatusDto.status, out ConsultationStatus status))
                return Result<ConsultationDto>.Failure(ConsultationError.InvalidStatus(updateStatusDto.status));

            consultation.Status = status;

            _unitOfWork.GetRepository<Consultation>().Update(consultation);

            await _unitOfWork.SaveChangesAsync();

            var doctor = await _userRepository.GetDoctorByIdAsync(DoctorId);
            var doctorName = doctor?.Fullname ?? $"Doctor {DoctorId}";

            await _notificationService.SendNotificationAsync( $"Your consultation request has been {status.ToString().ToLower()} by {doctorName}.", NotificationType.ConsultationUpdate, consultation.PatientId);

            return Result<ConsultationDto>.Success(_mapper.Map<ConsultationDto>(consultation));
        }
    }
}
