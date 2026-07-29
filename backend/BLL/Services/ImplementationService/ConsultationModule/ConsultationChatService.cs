using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Consultion;
using BLL.Hubs;
using BLL.Services.AbstractServices.ConsultationModule;
using DAL.Models.Consultation;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications.ConsultationSpecs;
using Microsoft.AspNetCore.SignalR;


namespace BLL.Services.ImplementationService.ConsultationModule
{
    public class ConsultationChatService (IUnitOfWork _unitOfWork , IMapper _mapper , IHubContext<ChatHub> _chatHub): IConsultationChatService
    {
        public async Task<Result<IEnumerable<ConsultationMessageDto>>> GetMessagesAsync(int consultationId, int requesterId)
        {
            var consultation = await ValidateConsultationAccessAsync(consultationId, requesterId);
           
            if (consultation.IsFailure)
                return Result<IEnumerable<ConsultationMessageDto>>.Failure(consultation.Error);

            if (consultation.Value.Status != ConsultationStatus.Accepted)
                return Result<IEnumerable<ConsultationMessageDto>>.Failure(ConsultationError.ConsultationNotActive());

            var messages = await _unitOfWork.GetRepository<ConsultationMessage>().GetAllAsync(new ConsultationMessagesSpecByConsultationId(consultationId));
            var massagesDto = _mapper.Map<IEnumerable<ConsultationMessageDto>>(messages);
            return Result< IEnumerable<ConsultationMessageDto>>.Success(massagesDto);
        }

        public async Task<Result<int>> GetUnreadCountAsync(int consultationId, int userId)
        {
            var res = await ValidateConsultationAccessAsync(consultationId, userId);
            if (res.IsFailure)
                return Result<int>.Failure(res.Error);

            var messages = await _unitOfWork.GetRepository<ConsultationMessage>()
                                            .GetAllAsync(new UnReadMessagesSpecs(consultationId,userId));
            return Result<int>.Success(messages.Count());
        }

        public async Task<Result> MarkMessagesAsReadAsync(int consultationId, int readerUserId)
        {
            
            var res =  await ValidateConsultationAccessAsync(consultationId,readerUserId);
            if (res.IsFailure)
                return Result.Failure(res.Error);
            var unread = await _unitOfWork.GetRepository<ConsultationMessage>().GetAllAsync(new UnReadMessagesSpecs(consultationId, readerUserId));
            foreach (var msg in unread)
            {
                msg.IsRead = true;
                _unitOfWork.GetRepository<ConsultationMessage>().Update(msg);
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<ConsultationMessageDto>> SendMessageAsync(int consultationId, int senderUserId, SendMessageDto dto)
        {
            var consultation = await ValidateConsultationAccessAsync(consultationId, senderUserId);
            if (consultation.IsFailure)
                return Result<ConsultationMessageDto>.Failure(consultation.Error);

            if (consultation.Value.Status != ConsultationStatus.Accepted)
                return Result<ConsultationMessageDto>.Failure(ConsultationError.ConsultationNotActive());

            var message = new ConsultationMessage
            {
                ConsultationId = consultationId,
                SenderUserId = senderUserId,
                Content = dto.Content,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<ConsultationMessage>().AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var savedMessage = (await _unitOfWork.GetRepository<ConsultationMessage>()
                                                 .GetAllAsync(new ConsultationMessageByIdSpec(message.Id))).FirstOrDefault();

            var messageDto = _mapper.Map<ConsultationMessageDto>(savedMessage);

            await _chatHub.Clients.Group($"consultation_{consultationId}")
                        .SendAsync("ReceiveMessage", messageDto);

            return Result<ConsultationMessageDto>.Success(messageDto);
        }


        private async Task<Result<Consultation>> ValidateConsultationAccessAsync(int consultationId,int userId)
        {
            var consultation = await _unitOfWork
                .GetRepository<Consultation>().GetByIdAsync(consultationId);
            if (consultation == null)
                return Result<Consultation>.Failure(ConsultationError.ConsultationNotFound(consultationId));


            if (consultation.PatientId != userId && consultation.DoctorId != userId)
                return Result<Consultation>.Failure(ConsultationError.UnauthorizedAccess());

            return Result<Consultation>.Success(consultation);
        }
    }
}
