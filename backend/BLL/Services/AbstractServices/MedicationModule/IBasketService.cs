using BLL.Abstractions;
using BLL.Dtos.Order;
using DomainLayer.Models.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.MedicationModule
{
    public interface IBasketService
    {
        Task<Result<BasketDto>> GetBasketAsync(int patientId);
        Task<Result<BasketDto>> AddItemAsync(int patientId, BasketItemInputDto dto);
        Task<Result<BasketDto>> UpdateItemQuantityAsync(int patientId, int medicationId, int quantity);
        Task<Result> RemoveItemAsync(int patientId, int medicationId);
        Task<Result> ClearBasketAsync(int patientId);
    }
}
