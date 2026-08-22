using BLL.Dtos.IdentityDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PresentationLayer.Controller;
using ServiceAbstractionLayer;
using Shared.DTOs.IdentityDtos;

namespace PL.Controllers
{
    public class AuthController(IAuthService _authenticationService ) : ApiControllerBase
    {
        
        [HttpPost("Login")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _authenticationService.LoginAsync(loginDto);
            if (user.IsFailure)
                return BadRequest(user.Error.Message);
            return Ok(user.Value);
        }

        [HttpPost("Register")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = await _authenticationService.RegisterAsync(model);
            if (user.IsFailure)
                return BadRequest(user.Error.Message);
            return Ok(user.Value);
        }

        #region ForgetPassword

        [HttpPost("forget-password")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {

            var token = await _authenticationService.GenerateResetTokenAsync(dto.Email);
            if (token.IsFailure)
                return BadRequest(new { message = "Invalid Email Address" });

            var resetLink = Url.Action("ResetPassword", "Auth",
                new { 
                    email = dto.Email,
                    token = token.Value 
                } , Request.Scheme);
             
            var sent = await _authenticationService.SendResetEmailAsync(dto.Email, resetLink!);
            if (sent.IsFailure)
                return StatusCode(500, new { message = "Failed to send email" });

            return Ok(new { message = "Reset link sent, please check your inbox." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {

            var result = await _authenticationService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);

            return Ok(new { message = "Password reset successfully." });
        }
        #endregion


    }
}
