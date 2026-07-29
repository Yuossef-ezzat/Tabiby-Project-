using BLL.Dtos;
using BLL.Services.AbstractServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;

namespace PL.Controllers
{
    [Authorize]
    public class ProfileUserController(IProfileUserService _profileUserService) : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.GetUserId();
            var result = await _profileUserService.GetProfileUserByIdAsync(userId);
            if (result.IsFailure)
                return NotFound();
            return Ok(result.Value);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfileUser dto)
        {
            var userId = User.GetUserId();
            var result = await _profileUserService.UpdateMyProfile(userId, dto);
            if (result.IsFailure)
                return NotFound();
            return Ok(result.Value);
        }
    }
}
