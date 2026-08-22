using BLL.Dtos.Order;
using BLL.Services.AbstractServices.MedicationModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PL.Extention;
using PresentationLayer.Controller;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PL.Controllers
{
    [Authorize]
    public class OrderController(IOrderService _orderService) : ApiControllerBase
    {
        #region Patient - Functionality
        [Authorize(Roles = "PATIENT")]
        [HttpGet("MyOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var result = await _orderService.GetMyOrdersAsync(User.GetUserId());

            return Ok(result.Value);
        }
        [Authorize(Roles = "PATIENT")]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteOrder(int OrderId)
        {
            var result = await  _orderService.DeleteOrderAsync(OrderId,User.GetUserId());
            if (!result.IsSuccess)
                return BadRequest(result.Error.Message);
            return NoContent();
        }

        [Authorize(Roles = "PATIENT")]
        [HttpGet("GetMyOrder/{orderId}")]
        public async Task<IActionResult> GetMyOrder(int orderId)
        {
            var result = await _orderService.GetOrderAsync(orderId, User.GetUserId());
            if (!result.IsSuccess)
                return NotFound(result.Error.Message);
            return Ok(result);
        }
        [Authorize(Roles = "PATIENT")]
        [HttpPost("Cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var result = await _orderService.CancelOrderAsync(orderId, User.GetUserId());
            if (!result.IsSuccess)
                return BadRequest(result.Error.Message);
            return Ok(result);
        }
        [Authorize(Roles = "PATIENT")]
        [HttpPost("Create")]
        [EnableRateLimiting("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            
            var result = await _orderService.CreateOrderAsync(User.GetUserId(), dto);
            if (!result.IsSuccess)
                return BadRequest(result.Error.Message);
            return Ok(result);
        }

        #endregion


        #region Pharmcist - Functionality 


        [Authorize(Roles = "PHARMACIST")]
        [HttpGet("Orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result.Value);
        }
        [Authorize(Roles = "PHARMACIST")]
        [HttpGet("GetOrder/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetOrderForMerchantAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Error.Message);
            return Ok(result);
        }
        [Authorize(Roles = "PHARMACIST")]
        [HttpPut("Orders/{orderId}/Status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatus dto)
        {

            var result = await _orderService.UpdateOrderStatusAsync(orderId, dto);
            if (!result.IsSuccess)
                return BadRequest(result.Error.Message);

            return Ok(result);
        }
        [Authorize(Roles = "PHARMACIST")]
        [HttpDelete("Pharmacist/Delete")]
        public async Task<IActionResult> DeleteByPharmacistOrder(int OrderId)
        {
            var result = await _orderService.DeleteOrderByPharmacistAsync(OrderId);
            if (!result.IsSuccess)
                return BadRequest(result.Error.Message);
            return NoContent();
        }
        #endregion

    }
}
    

