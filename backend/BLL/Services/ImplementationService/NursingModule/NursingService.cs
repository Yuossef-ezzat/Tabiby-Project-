using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Consultion;
using BLL.Dtos.Nursing;
using BLL.Services.AbstractServices;
using BLL.Services.AbstractServices.Users;
using DAL.Models.NursingModule;
using DAL.Models.Users;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications.NursingRequestSpecs;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services.ImplementationService.NursingModule
{
    public class NursingService(IUnitOfWork _unitOfWork, IMapper _mapper, INotificationService _notificationService, IUserRepository _userRepository, UserManager<ApplicationUser> _userManager) : INursingService
    {

        public async Task<Result<IEnumerable<NurseInfoDto>>> SearchNursesAsync(SearchNurseDto searchDto)
        {
            var nurses = await _userRepository.SearchNursesAsync(
                searchDto.Name,
                searchDto.Specialization,
                searchDto.PageNumber,
                searchDto.PageSize);


            return Result<IEnumerable<NurseInfoDto>>.Success(_mapper.Map<IEnumerable<NurseInfoDto>>(nurses));
        }

        public async Task<Result<NursingRequestDto>> RequestNursingAsync(int patientId, CreateNursingRequestDto dto)
        {

            var nurse = await _userManager.FindByIdAsync(dto.NurseId.ToString())
                        as Nurse; 
            if(nurse is null)
                return Result<NursingRequestDto>.Failure(NurseError.NotFound(dto.NurseId));

            if (!nurse.IsActive)
                return Result<NursingRequestDto>.Failure(NurseError.InActive(dto.NurseId));

            var activeRequests = await _unitOfWork
                        .GetRepository<NursingRequest>()
                        .GetAllAsync(new RequestByUserIdAndNurseId(patientId, dto.NurseId));

            if (activeRequests.Any(r => r.Status == "Pending" || r.Status == "Accepted"))
                return Result<NursingRequestDto>.Failure(NurseError.ActiveRequestExists());

            var request = new NursingRequest
            {
                PatientId = patientId,
                NurseId = dto.NurseId,
                CareType = dto.CareType,
                Status = "Pending",
                RequestedDate = DateTime.UtcNow,

            };

            await _unitOfWork.GetRepository<NursingRequest>().AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _userManager.FindByIdAsync(patientId.ToString());
            var patientName = patient?.Fullname ?? $"patient {patientId}";

            await _notificationService.SendNotificationAsync(
                $"You have a new nursing request from {patientName}.",
                NotificationType.System,
                dto.NurseId
            );

            return Result<NursingRequestDto>.Success(_mapper.Map<NursingRequestDto>(request));
        }

        public async Task<Result<IEnumerable<NursingRequestDto>>> GetMyNursingRequestsAsync(int requesterId)
        {
            var myRequests = await _unitOfWork.GetRepository<NursingRequest>().GetAllAsync(new MyNursingRequestsSpecs(requesterId));
            if (myRequests is null || !myRequests.Any())
                return Result<IEnumerable<NursingRequestDto>>.Success(Enumerable.Empty<NursingRequestDto>());

            return Result<IEnumerable<NursingRequestDto>>.Success( _mapper.Map<IEnumerable<NursingRequestDto>>(myRequests));

        }

        public async Task<Result<NursingRequestDto>> UpdateNursingStatusAsync(int requestId, UpdateNursingStatusDto dto, int userId)
        {
            var request = (await _unitOfWork.GetRepository<NursingRequest>()
                .GetAllAsync(new NursingRequestByIdSpec(requestId)))
                .FirstOrDefault();
            if (request is null)
                return Result<NursingRequestDto>.Failure(NurseError.RequestNotFound(requestId));

            if (request.NurseId != userId)
                return Result<NursingRequestDto>.Failure(NurseError.UnauthorizedNursingAccess(userId, requestId));

            if (request!.Status == "Cancelled" || request!.Status == "Completed")
                return Result<NursingRequestDto>.Failure(NurseError.InvalidStatus(request.Status));

            request.Status = dto.Status;

            _unitOfWork.GetRepository<NursingRequest>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            var nurse = await _userManager.FindByIdAsync(userId.ToString());
            var nurseName = nurse?.Fullname ?? $"nurse {userId}";

            await _notificationService.SendNotificationAsync(
                $"Your nursing request status has been updated to '{dto.Status}' by {nurseName}.",
                NotificationType.System,
                request.PatientId
            );

            return Result<NursingRequestDto>.Success(_mapper.Map<NursingRequestDto>(request));
        }

        public async Task<Result<NursingRequestDto>> CancelNursingAsync(int requestId, int userId)
        {
            var request = await _unitOfWork.GetRepository<NursingRequest>().GetByIdAsync(requestId);
            if (request is null)
                return Result<NursingRequestDto>.Failure(NurseError.RequestNotFound(requestId));
            if (request.PatientId != userId)
                return Result<NursingRequestDto>.Failure(NurseError.UnauthorizedNursingAccess(userId, requestId));
            if (request.Status != "Pending")
                return Result<NursingRequestDto>.Failure(NurseError.NotCancelable(requestId));

            request.Status = "Cancelled";

            _unitOfWork.GetRepository<NursingRequest>().Update(request);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _userManager.FindByIdAsync(userId.ToString());
            var patientName = patient?.Fullname ?? $"patient {userId}";

            await _notificationService.SendNotificationAsync(
                $"Your nursing request has been cancelled by {patientName}.",
                NotificationType.System,
                request.NurseId
            );

            return Result<NursingRequestDto>.Success(_mapper.Map<NursingRequestDto>(request));
        }

        public async Task<Result<NursingReviewDto>> AddNursingReviewAsync(int requestId, int patientId, CreateNursingReviewDto dto)
        {
            var request = (await _unitOfWork.GetRepository<NursingRequest>()
                .GetAllAsync(new NursingRequestByIdSpec(requestId)))
                .FirstOrDefault();
            if (request == null)
                return Result<NursingReviewDto>.Failure(NurseError.RequestNotFound(requestId));

            if (request.PatientId != patientId)
                return Result<NursingReviewDto>.Failure(NurseError.UnauthorizedNursingAccess(patientId, requestId));

            if (request.Status != "Completed")
                return Result<NursingReviewDto>.Failure(NurseError.NotReviewable(requestId));
            if (request.Review != null)
                return Result<NursingReviewDto>.Failure(NurseError.NotReviewable(requestId));

            var review = new NursingReview
            {
                NursingRequestId = requestId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _unitOfWork.GetRepository<NursingReview>().AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var patient = await _userManager.FindByIdAsync(patientId.ToString());
            var patientName = patient?.Fullname ?? $"patient {patientId}";

            await _notificationService.SendNotificationAsync(
                $"You received a new review with rating {dto.Rating}/5 from {patientName}.",
                NotificationType.System,
                request.NurseId
            );

            return Result<NursingReviewDto>.Success(_mapper.Map<NursingReviewDto>(review));
        }

        public async Task<Result<IEnumerable<NursingReviewDto>>> GetNursingReviewAsync(int requestId)
        {
            
            var request = await _unitOfWork.GetRepository<NursingRequest>().GetByIdAsync(requestId);
            if (request is null)
                return Result<IEnumerable<NursingReviewDto>>.Failure(NurseError.RequestNotFound(requestId));

            var review = await _unitOfWork.GetRepository<NursingReview>().GetAllAsync(new ReviewByRequestIdSpecs(requestId));

            return Result<IEnumerable<NursingReviewDto>>.Success(_mapper.Map<IEnumerable<NursingReviewDto>>(review));
        }

    }
}
