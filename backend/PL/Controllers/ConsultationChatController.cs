using BLL.Dtos.Consultion;
using BLL.Services.AbstractServices.ConsultationModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize(Roles = "PATIENT,DOCTOR")]
    public class ConsultationChatController(IConsultationChatService _chatService) : ApiControllerBase
    {
        [HttpGet("{consultationId}/messages")]
        public async Task<IActionResult> GetMessages(int consultationId)
        {
            var result = await _chatService.GetMessagesAsync(consultationId, User.GetUserId());
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpPost("{consultationId}/messages")]
        public async Task<IActionResult> SendMessage(int consultationId, [FromBody] SendMessageDto dto)
        {

            var result = await _chatService.SendMessageAsync(consultationId, User.GetUserId(), dto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpPost("{consultationId}/read")]
        public async Task<IActionResult> MarkAsRead(int consultationId)
        {
            var result =  await _chatService.MarkMessagesAsReadAsync(consultationId, User.GetUserId());
            if(result.IsFailure)
                return BadRequest(result.Error.Message);
            return NoContent();
        }

        [HttpGet("{consultationId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(int consultationId)
        {
            var result = await _chatService.GetUnreadCountAsync(consultationId, User.GetUserId());
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }
    }
}
