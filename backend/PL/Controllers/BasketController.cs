using BLL.Abstractions;
using BLL.Dtos.Order;
using BLL.Services.AbstractServices.MedicationModule;
using BLL.Services.ImplementationService.MedicationModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;

namespace PL.Controllers
{
    [Authorize(Roles = "PATIENT")]
    public class BasketController(IBasketService _basketService) : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {
            var result = await _basketService.GetBasketAsync(User.GetUserId());
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] BasketItemInputDto dto)
        {

            var result = await _basketService.AddItemAsync(User.GetUserId(), dto);
            if(result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpPut("UpdateItem/{medicationId}")]
        public async Task<IActionResult> UpdateItem(int medicationId, [FromBody] int quantity)
        {
            var result = await _basketService.UpdateItemQuantityAsync(User.GetUserId(), medicationId, quantity);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return Ok(result.Value);
        }

        [HttpDelete("RemoveItem/{medicationId}")]
        public async Task<IActionResult> RemoveItem(int medicationId)
        {
            var result = await _basketService.RemoveItemAsync(User.GetUserId(), medicationId);
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return NoContent();
        }

        [HttpDelete("Clear")]
        public async Task<IActionResult> ClearBasket()
        {
            var result = await _basketService.ClearBasketAsync(User.GetUserId());
            if (result.IsFailure)
                return BadRequest(result.Error.Message);
            return NoContent();
        }
    }

}
