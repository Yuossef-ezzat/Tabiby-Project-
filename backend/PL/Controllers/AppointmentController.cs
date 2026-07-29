using BLL.Dtos;
using BLL.Dtos.Appointment;
using BLL.Dtos.Schedule;
using BLL.Services.AbstractServices;
using BLL.Services.AbstractServices.AppointmentModule;
using DAL.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;
using System;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize]
    public class AppointmentController(
        IAppointmentService _appointmentService
                        ) : ApiControllerBase
    {
        #region Patient - Functionality

        [Authorize(Roles = "PATIENT")]
        [HttpPost("Book")]
        public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto dto)
        {
            var appointment = await _appointmentService.BookAppointmentAsync(User.GetUserId(), dto);
            if (!appointment.IsSuccess)
            {
                return BadRequest(appointment.Error);
            }
            return Ok(appointment.Value);
        }

        [Authorize(Roles = "PATIENT")]
        [HttpGet("MyAppointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var appointments = await _appointmentService.GetMyAppointmentsAsync(User.GetUserId());
            if (!appointments.IsSuccess)
            {
                return BadRequest(appointments.Error);
            }
            return Ok(appointments.Value);
        }

        [Authorize(Roles = "PATIENT")]
        [HttpPut("Update/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointment(int appointmentId, [FromBody] UpdateAppointmentDto dto)
        {
            var appointment = await _appointmentService.UpdateAppointmentAsync(appointmentId, dto, User.GetUserId());
            if (!appointment.IsSuccess)
            {
                return BadRequest(appointment.Error);
            }
            return Ok(appointment.Value);
        }

        #endregion

        #region Shared - Patient & Doctor

        [HttpGet("GetAppointment/{appointmentId}")]
        public async Task<IActionResult> GetAppointment(int appointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentAsync(appointmentId, User.GetUserId());
            if (appointment.IsFailure)
                return BadRequest(appointment.Error.Message);
            return Ok(appointment.Value);
        }

        [HttpPost("Cancel/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _appointmentService.CancelAppointmentAsync(appointmentId, User.GetUserId());
            if(appointment.IsFailure)
                return BadRequest(appointment.Error);
            return Ok(appointment.Value);
        }

        #endregion

        #region Doctor - Functionality

        [Authorize(Roles = "DOCTOR")]
        [HttpGet("DoctorAppointments")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var appointments = await _appointmentService.GetDoctorAppointmentsAsync(User.GetUserId());
            if (appointments.IsFailure)
                return BadRequest(appointments.Error.Message);

            return Ok(appointments.Value);
        }

        [Authorize(Roles = "DOCTOR")]
        [HttpPost("Confirm/{appointmentId}")]
        public async Task<IActionResult> ConfirmAppointment(int appointmentId)
        {
            var appointment = await _appointmentService.ConfirmAppointmentAsync(appointmentId, User.GetUserId());
            if (appointment.IsFailure)
                return BadRequest(appointment.Error.Message);
            return Ok(appointment.Value);
        }

        [Authorize(Roles = "DOCTOR")]
        [HttpPost("Complete/{appointmentId}")]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            var appointment = await _appointmentService.CompleteAppointmentAsync(appointmentId, User.GetUserId());
            if (appointment.IsFailure)
                return BadRequest(appointment.Error.Message);
            return Ok(appointment.Value);
        }

        #endregion

        #region Public

        [AllowAnonymous]
        [HttpGet("AvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);
            if (slots.IsFailure)
                return BadRequest(slots.Error.Message);
            return Ok(slots.Value);
        }

        #endregion
    }
}
