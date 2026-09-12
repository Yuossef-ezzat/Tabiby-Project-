using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Order;
using BLL.Services.AbstractServices.MedicationModule;
using DAL.Models.OrderModule;
using DAL.Models.Users;
using DAL.Repository;
using DAL.Shared.Enums;
using DAL.Specifications.OrderSpecs;
using DomainLayer.Models.BasketModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.ImplementationService.MedicationModule
{
    public class OrderService(IUnitOfWork _unitOfWork, IMapper _mapper, IUserRepository _userRepository) : IOrderService
    {
        public async Task<Result<IEnumerable<OrderDto>>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetAllAsync(new AllOrdersSpecs());
            if (orders == null)
                return Result<IEnumerable<OrderDto>>.Success(Enumerable.Empty<OrderDto>());
            return Result< IEnumerable < OrderDto >>.Success( _mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        public async Task<Result<OrderDto>> CancelOrderAsync(int orderId, int patientId)
        {
            var order = (await _unitOfWork.GetRepository<Order>()
                                .GetAllAsync(new OrderByOrderIdAndPatientIdSpecs(orderId, patientId))).FirstOrDefault();
            if (order == null)
                return Result<OrderDto>.Failure(OrderError.NotFound(orderId));

            if (order.Status != OrderStatus.Pending)
                return Result<OrderDto>.Failure(OrderError.NotCancelable(orderId));

            var patient = await _userRepository.GetPatientWithBasketAsync(patientId);
            if (patient is null)
                return Result<OrderDto>.Failure(PatientError.PatientNotFound(patientId));

            #region Restore stock
            
            await RestoreStock(order);

            #endregion

            order.Status = OrderStatus.Cancelled;

            await _unitOfWork.SaveChangesAsync();
            var orderDto = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Success(orderDto);

        }
        public async Task<Result> DeleteOrderAsync(int OrderId , int PatientId)
        {
            var Order = (await _unitOfWork.GetRepository<Order>()
                                   .GetAllAsync(new OrderByOrderIdAndPatientIdSpecs(OrderId, PatientId))).FirstOrDefault();
            if (Order == null)
                return Result.Failure(OrderError.NotFound(OrderId));

            var allowedStatuses = new[]
            {
                OrderStatus.Cancelled,
                OrderStatus.Rejected,
                OrderStatus.Delivered
            };

            if (!allowedStatuses.Contains(Order.Status))
            {
                return Result.Failure(OrderError.OrderCantBeDeleted(OrderId));
            }
            _unitOfWork.GetRepository<Order>().Delete(Order);
             await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result> DeleteOrderByPharmacistAsync(int OrderId)
        {
            var Order = (await _unitOfWork.GetRepository<Order>()
                                   .GetAllAsync(new OrderByOrderIdSpecs(OrderId))).FirstOrDefault();

            if (Order == null)
                return Result.Failure(OrderError.NotFound(OrderId));

            var allowedStatuses = new[]
            {
                OrderStatus.Cancelled,
                OrderStatus.Rejected,
                OrderStatus.Delivered
            };

            if (!allowedStatuses.Contains(Order.Status))
            {
                return Result.Failure(OrderError.OrderCantBeDeleted(OrderId));
            }
            _unitOfWork.GetRepository<Order>().Delete(Order);
             await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result<OrderDto>> CreateOrderAsync(int patientId, CreateOrderDto dto)
        {
            var Address = _mapper.Map<OrderAddress>(dto.Address);

            var patient = await _userRepository.GetPatientWithBasketAsync(patientId);
            if (patient is null)
                return Result<OrderDto>.Failure(PatientError.PatientNotFound(patientId));

            if(patient.Basket == null)
                return Result<OrderDto>.Failure(BasketError.NotFound(patientId));

            var Basket = (await _unitOfWork.GetRepository<CustomerBasket>()
                                            .GetAllAsync(new BasketByIdSpecs(patient!.Basket!.Id))).FirstOrDefault();
            if (Basket == null)
                return Result<OrderDto>.Failure(BasketError.NotFound(patient!.Basket!.Id));

            if (!Basket.BasketItems.Any())
                return Result<OrderDto>.Failure(BasketError.BasketEmpty(patient!.Basket!.Id));
            List<OrderItem> orderItems = new List<OrderItem>();

            var medicationRepo = _unitOfWork.GetRepository<Medication>();

            var medicationIds = Basket.BasketItems.Select(b=>b.MedicationId).ToList();
            var medications = await medicationRepo.GetAllAsync(new MedicationsByIdsSpec(medicationIds));
            var medicationsDict = medications.ToDictionary(x => x.Id);

            foreach (var basketItem in Basket.BasketItems)
            {
                if (!medicationsDict.TryGetValue(basketItem.MedicationId, out var medication))
                    return Result<OrderDto>.Failure(
                        MedicationError.NotFound(basketItem.MedicationId));

                if (medication.Stock < basketItem.Quantity)
                    return Result<OrderDto>.Failure(
                        BasketError.InsufficientStock(medication.Name, medication.Stock));

                var OrderItem = new OrderItem()
                {

                    MedicationId = basketItem.MedicationId,
                    Name = medication.Name,
                    PictureUrl = medication.PictureUrl ?? "",
                    UnitPrice = medication.Price,
                    Quantity = basketItem.Quantity,
                };

                orderItems.Add(OrderItem);
                medication.Stock -= basketItem.Quantity;

            }

            var SubTotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
            var Order = new Order()
            {
                PatientId = patientId,
                SubTotal = SubTotal,
                Address = Address,
                OrderItem = orderItems,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
            };

            await _unitOfWork.GetRepository<Order>().AddAsync(Order);
            
            var itemRepo = _unitOfWork.GetRepository<BasketItem>();
            foreach (var item in Basket.BasketItems.ToList())
            {
                itemRepo.Delete(item);
            }

            await _unitOfWork.SaveChangesAsync();

            var orderDto =  _mapper.Map<OrderDto>(Order);
            return Result<OrderDto>.Success(orderDto);

        }

        public async Task<Result<IEnumerable<OrderDto>>> GetMyOrdersAsync(int patientId)
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetAllAsync(new OrdersByPatientIdSpecs(patientId));
            if (orders == null || !orders!.Any())
                return Result<IEnumerable<OrderDto>>.Success(Enumerable.Empty<OrderDto>());
            var orderDto =  _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Result<IEnumerable<OrderDto>>.Success(orderDto);
        }

        public async Task<Result<OrderDto>> GetOrderAsync(int orderId, int patientId)
        {
            var order = (await _unitOfWork.GetRepository<Order>()
                        .GetAllAsync(new OrderByOrderIdAndPatientIdSpecs(orderId, patientId))).FirstOrDefault();
            if (order == null)
                return Result<OrderDto>.Failure(OrderError.NotFound(orderId));

            var orderDto = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Success(orderDto);
        }
        public async Task<Result<OrderDto>> GetOrderForMerchantAsync(int orderId)
        {
            var order = (await _unitOfWork.GetRepository<Order>()
                .GetAllAsync(new OrderByIdSpec(orderId)))
                .FirstOrDefault();
            if (order == null)
                return Result<OrderDto>.Failure(OrderError.NotFound(orderId));

            return Result<OrderDto>.Success( _mapper.Map<OrderDto>(order));
        }
        public async Task<Result<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatus dto)
        {
            var order = (await _unitOfWork.GetRepository<Order>()
                .GetAllAsync(new OrderByIdSpec(orderId)))
                .FirstOrDefault();
            if (order == null)
                return Result<OrderDto>.Failure(OrderError.NotFound(orderId));

            if (order.Status == OrderStatus.Pending && dto.Status == OrderStatus.Rejected)
            {
                await RestoreStock(order);
            }

            order.Status = dto.Status;

            await _unitOfWork.SaveChangesAsync();

            return Result<OrderDto>.Success( _mapper.Map<OrderDto>(order));
        }
        private async Task RestoreStock(Order order)
        {
            var medicationRepo = _unitOfWork.GetRepository<Medication>();
            var ids = order.OrderItem.Select(x => x.MedicationId).ToList();

            var meds = await medicationRepo.GetAllAsync(new MedicationsByIdsSpec(ids));

            var dict = meds.ToDictionary(x => x.Id);

            foreach (var item in order.OrderItem)
            {
                if (dict.TryGetValue(item.MedicationId, out var medication))
                {
                    medication.Stock += item.Quantity;
                }
            }
        }

    }
}
