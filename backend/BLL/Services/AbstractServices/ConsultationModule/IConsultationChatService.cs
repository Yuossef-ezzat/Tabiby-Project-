using BLL.Abstractions;
using BLL.Dtos.Consultion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.ConsultationModule
{
    public interface IConsultationChatService
    {
        Task<Result<IEnumerable<ConsultationMessageDto>>> GetMessagesAsync(int consultationId, int requesterId);
        Task<Result<ConsultationMessageDto>> SendMessageAsync(int consultationId, int senderUserId, SendMessageDto dto);
        Task<Result> MarkMessagesAsReadAsync(int consultationId, int readerUserId);

        Task<Result<int>> GetUnreadCountAsync(int consultationId, int userId);
    }
}
