using BLL.Dtos.Nursing;
using BLL.Services.AbstractServices.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize]
    public class NursingController(INursingService _nursingService) : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchNurses([FromQuery] SearchNurseDto searchDto)
        {
            var result = await _nursingService.SearchNursesAsync(searchDto);
            return Ok(result.Value);
        }

        [Authorize(Roles = "PATIENT")]
        [HttpPost("Request")]
        public async Task<IActionResult> RequestNursing([FromBody] CreateNursingRequestDto dto)
        {
            var result = await _nursingService.RequestNursingAsync(User.GetUserId(), dto);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [Authorize(Roles = "PATIENT,NURSE")]
        [HttpGet("MyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var result = await _nursingService.GetMyNursingRequestsAsync(User.GetUserId());
            return Ok(result.Value);
        }

        [Authorize(Roles = "NURSE,ADMIN")]
        [HttpPut("UpdateStatus/{requestId}")]
        public async Task<IActionResult> UpdateStatus(int requestId, [FromBody] UpdateNursingStatusDto dto)
        {

            var result = await _nursingService.UpdateNursingStatusAsync(requestId, dto, User.GetUserId());
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [Authorize(Roles = "PATIENT,NURSE")]
        [HttpPost("Cancel/{requestId}")]
        public async Task<IActionResult> CancelNursing(int requestId)
        {
            var result = await _nursingService.CancelNursingAsync(requestId, User.GetUserId());
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [Authorize(Roles = "PATIENT")]
        [HttpPost("Review/{requestId}")]
        public async Task<IActionResult> AddReview(int requestId, [FromBody] CreateNursingReviewDto dto)
        {

            var result = await _nursingService.AddNursingReviewAsync(requestId, User.GetUserId(), dto);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("Review/{requestId}")]
        public async Task<IActionResult> GetReview(int requestId)
        {
            var result = await _nursingService.GetNursingReviewAsync(requestId);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
