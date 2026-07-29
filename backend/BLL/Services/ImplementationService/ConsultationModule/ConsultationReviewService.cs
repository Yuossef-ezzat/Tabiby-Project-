using AutoMapper;
using BLL.Dtos.Consultion;
using BLL.Services.AbstractServices;
using BLL.Services.AbstractServices.ConsultationModule;
using DAL.Models.Consultation;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications.ConsultationSpecs;
using Microsoft.AspNetCore.Identity;
using DAL.Models.Users;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;

namespace BLL.Services.ImplementationService.ConsultationModule
{
    public class ConsultationReviewService(IUnitOfWork _unitOfWork ,
        IMapper _mapper , INotificationService _notificationService, UserManager<ApplicationUser> _userManager) : IConsultationReviewService
    {
        public async Task<Result<ConsultationReviewDto>> AddReviewAsync(int consultationId,int patientId, CreateConsultationReviewDto dto)
        {
            var consultation = await _unitOfWork.GetRepository<Consultation>()
                                                .GetByIdAsync(consultationId);
            if (consultation == null)
                return Result<ConsultationReviewDto>.Failure(ConsultationError.ConsultationNotFound(consultationId));

            if (consultation.PatientId != patientId)
                return Result<ConsultationReviewDto>.Failure(ConsultationError.UnauthorizedAccess());

            if (consultation.Status != ConsultationStatus.Completed)
                return Result<ConsultationReviewDto>.Failure(ConsultationError.ConsultationNotCompleted());

            var repo = _unitOfWork.GetRepository<ConsultationReview>();

            var existingReview = (await repo.GetAllAsync(new ReviewByConsultationIdSpec(consultationId))).FirstOrDefault();
            
            if (existingReview != null)
                return Result<ConsultationReviewDto>.Failure(ConsultationError.ConsultationReviewAlreadyExists());

            var review = _mapper.Map<ConsultationReview>(dto);
            
            review.ConsultationId = consultationId;

            await repo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync(); 

            var patient = await _userManager.FindByIdAsync(patientId.ToString());
            var patientName = patient?.Fullname ?? $"patient {patientId}";

            await _notificationService.SendNotificationAsync( $"You received a new review with rating {dto.Rating}/5 from {patientName}.", NotificationType.ConsultationReview, consultation.DoctorId);
           var consultationReviewDto = _mapper.Map<ConsultationReviewDto>(review);
            return Result<ConsultationReviewDto>.Success(consultationReviewDto);
        }

        public async Task<Result<ConsultationReviewDto?>> GetConsultationReviewsByConsultationId(int consultationId)
        {
            var repo = _unitOfWork.GetRepository<ConsultationReview>();

            var review = (await repo.GetAllAsync(new ReviewByConsultationIdSpec(consultationId))).FirstOrDefault();

            if (review is null)
                return Result<ConsultationReviewDto?>.Failure(ConsultationError.ConsultationReviewNotFound(consultationId));

            var consultationReviewDto = _mapper.Map<ConsultationReviewDto>(review);
            return Result<ConsultationReviewDto?>.Success(consultationReviewDto);
        }
    }
}
