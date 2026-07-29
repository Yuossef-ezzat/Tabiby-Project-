using BLL.Abstractions;
using BLL.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.MedicationModule
{
    public interface IOrderService
    {
        Task<Result<IEnumerable<OrderDto>>> GetAllOrdersAsync();
        Task<Result<OrderDto>> GetOrderAsync(int orderId, int patientId);
        Task<Result<IEnumerable<OrderDto>>> GetMyOrdersAsync(int patientId);
        Task<Result<OrderDto>> GetOrderForMerchantAsync(int orderId);
        Task<Result> DeleteOrderAsync(int OrderId, int PatientId);
        Task<Result> DeleteOrderByPharmacistAsync(int OrderId);

        Task<Result<OrderDto>> CreateOrderAsync(int patientId, CreateOrderDto dto);
        Task<Result<OrderDto>> CancelOrderAsync(int orderId, int patientId);
        Task<Result<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatus dto);


    }
}
