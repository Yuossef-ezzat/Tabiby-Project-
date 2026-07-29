using BLL.Dtos.Consultion;
using BLL.Services.AbstractServices.ConsultationModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize]
    public class ConsultationReviewController(IConsultationReviewService _reviewService) : ApiControllerBase
    {
        [Authorize(Roles = "PATIENT")]
        [HttpPost("{consultationId}")]
        public async Task<IActionResult> AddReview(int consultationId, [FromBody] CreateConsultationReviewDto dto)
        {
            
            var result = await _reviewService.AddReviewAsync(consultationId, User.GetUserId(), dto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpGet("{consultationId}")]
        public async Task<IActionResult> GetReview(int consultationId)
        {
            var result = await _reviewService.GetConsultationReviewsByConsultationId(consultationId);
            if(result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }
    }
}
