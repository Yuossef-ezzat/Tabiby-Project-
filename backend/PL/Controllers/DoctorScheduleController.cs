using BLL.Dtos.Schedule;
using BLL.Services.AbstractServices.AppointmentModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize(Roles = "DOCTOR")]
    public class DoctorScheduleController(IDoctorScheduleService _scheduleService) : ApiControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateDoctorScheduleDto dto)
        {

            var result = await _scheduleService.CreateScheduleAsync(User.GetUserId(), dto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpGet("Doctor")]
        public async Task<IActionResult> GetDoctorSchedules()
        {   
            var result = await _scheduleService.GetDoctorSchedulesAsync(User.GetUserId());
            if (result.IsFailure)
                return NotFound(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpPut("{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule(int scheduleId, [FromBody] UpdateDoctorScheduleDto dto)
        {

            var result = await _scheduleService.UpdateScheduleAsync(scheduleId, dto, User.GetUserId());
            if (result.IsFailure)
                return NotFound(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(int scheduleId)
        {
            var result = await _scheduleService.DeleteScheduleAsync(scheduleId, User.GetUserId());
            if (result.IsFailure)
                return NotFound(result.Error.Message);

            return NoContent();
        }
    }
}
