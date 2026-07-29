using BLL.Dtos.Consultion;
using BLL.Dtos.Doctor;
using BLL.Services.AbstractServices.ConsultationModule;
using BLL.Services.AbstractServices.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;
using System.Threading.Tasks;

namespace PL.Controllers
{
    public class ConsultationController(IConsultationService _consultationService) : ApiControllerBase
    {
        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors([FromQuery] SearchDoctorDto searchDto)
        {
            var result = await _consultationService.SearchDoctorsAsync(searchDto);
            return Ok(result.Value);
        }
        [Authorize(Roles = "PATIENT")]
        [HttpPost("RequestConsultation")]
        public async Task<IActionResult> RequestConsultation([FromBody] CreateConsultationDto createConsultationDto)
        {

            var result = await _consultationService.RequestConsultationAsync(User.GetUserId(), createConsultationDto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }
        [Authorize(Roles = "PATIENT,DOCTOR")]
        [HttpGet("MyConsultations")]
        public async Task<IActionResult> MyConsultations()
        {
            var result = await _consultationService.GetMyConsultationsAsync(User.GetUserId());
            if(result.IsFailure)
                return NotFound(result.Error.Message);
            return Ok(result.Value);
        }

        [Authorize(Roles = "PATIENT,DOCTOR")]
        [HttpGet("GetMyConsultationById/{id}")]
        public async Task<IActionResult> GetMyConsultationById(int id)
        {
            var result = await _consultationService.GetConsultationByIdAsync(id, User.GetUserId());
            if(result.IsFailure)
                return NotFound(result.Error.Message);
            return Ok(result.Value);
        }
        [Authorize(Roles = "DOCTOR")]
        [HttpDelete("DeleteConsultation/{id}")]
        public async Task<IActionResult> DeleteConsultation(int id)
        {
            var result = await _consultationService.DeleteConsultationAsync(id, User.GetUserId());
            if(result.IsFailure)
                return BadRequest(result.Error.Message);
            return NoContent();
            
        }
        [Authorize(Roles = "PATIENT,DOCTOR")]
        [HttpPut("UpdateConsultationStatus/{id}")]
        public async Task<IActionResult> UpdateConsultationStatus(int id, [FromBody] UpdateConsultionStatusDto updateStatusDto)
        {
            var result = await _consultationService.UpdateConsultationStatusAsync(id, User.GetUserId(), updateStatusDto);
            if(result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }
    }
}
