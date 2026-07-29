using BLL.Dtos.Admin;
using BLL.Services.AbstractServices.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController(IAdminService _adminService) : ApiControllerBase
    {

        [HttpPost("Doctors")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorAdminDto dto)
        {
            var result = await _adminService.CreateDoctorAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return StatusCode(201, result.Value);
        }

        [HttpPost("Nurses")]
        public async Task<IActionResult> CreateNurse([FromBody] CreateNurseAdminDto dto)
        {
            var result = await _adminService.CreateNurseAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return StatusCode(201, result.Value);
        }

        [HttpPost("Pharmacists")]
        public async Task<IActionResult> CreatePharmacist([FromBody] CreatePharmacistAdminDto dto)
        {
            var result = await _adminService.CreatePharmacistAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return StatusCode(201, result.Value);
        }

        [HttpDelete("Users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var requestingAdminId = User.GetUserId();
            var result = await _adminService.DeleteUserAsync(userId, requestingAdminId);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

       

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] SearchUserDto searchDto)
        {
            var result = await _adminService.GetAllUsersAsync(searchDto);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(new
            {
                TotalCount = result.Value.TotalCount,
                Users = result.Value.Users
            });
        }

        [HttpPut("Users/{userId}/ToggleActive")]
        public async Task<IActionResult> ToggleUserActive(int userId)
        {
            var requestingAdminId = User.GetUserId();
            var result = await _adminService.ToggleUserActiveStatusAsync(userId, requestingAdminId);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpGet("Specialties")]
        public async Task<IActionResult> GetSpecialties()
        {
            var result = await _adminService.GetSpecialtiesAsync();
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }
    }
}
