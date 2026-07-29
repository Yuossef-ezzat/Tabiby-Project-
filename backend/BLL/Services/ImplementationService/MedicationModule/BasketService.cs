using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Order;
using BLL.Services.AbstractServices.MedicationModule;
using DAL.Models.OrderModule;
using DAL.Repository;
using DAL.Specifications.OrderSpecs;
using DomainLayer.Models.BasketModule;

namespace BLL.Services.ImplementationService.MedicationModule
{
    public class BasketService(IUnitOfWork _unitOfWork, IMapper _mapper) : IBasketService
    {
        public async Task<Result<BasketDto>> GetBasketAsync(int patientId)
        {
            var spec = new BasketByUserIdSpecs(patientId);
            var basket = (await _unitOfWork.GetRepository<CustomerBasket>().GetAllAsync(spec)).FirstOrDefault();

            if (basket is null)
                return Result<BasketDto>.Failure(BasketError.NotFound(patientId));
            var basketDto = _mapper.Map<BasketDto>(basket);
            return Result<BasketDto>.Success(basketDto);
        }

        public async Task<Result<BasketDto>> AddItemAsync(int patientId, BasketItemInputDto dto)
        {
            var medication = await _unitOfWork.GetRepository<Medication>().GetByIdAsync(dto.MedicationId);

            if (medication is null)
                return Result<BasketDto>.Failure(MedicationError.NotFound(dto.MedicationId));
            if (!medication.IsAvailable || medication.Stock < dto.Quantity)
                return Result<BasketDto>.Failure(BasketError.InsufficientStock(medication.Name,medication.Stock));

            var basket = (await _unitOfWork.GetRepository<CustomerBasket>()
                            .GetAllAsync(new BasketByUserIdSpecs(patientId))).FirstOrDefault();

            if (basket is null)
            {
                basket = new CustomerBasket { PatientId = patientId };
                await _unitOfWork.GetRepository<CustomerBasket>().AddAsync(basket);
            }

            var existingItem = basket.BasketItems
                .FirstOrDefault(i => i.MedicationId == dto.MedicationId);

            if (existingItem is not null)
                existingItem.Quantity += dto.Quantity;
            else
                basket.BasketItems.Add(new BasketItem
                {
                    MedicationId = dto.MedicationId,
                    Price = medication.Price,
                    PictureUrl = medication.PictureUrl ?? "",
                    Quantity = dto.Quantity
                });

            await _unitOfWork.SaveChangesAsync();
            var basketDto = _mapper.Map<BasketDto>(basket);
            return Result<BasketDto>.Success(basketDto);
        }

        public async Task<Result<BasketDto>> UpdateItemQuantityAsync(int patientId, int medicationId, int quantity)
        {
            var spec = new BasketByUserIdSpecs(patientId);
            var basket = (await _unitOfWork.GetRepository<CustomerBasket>().GetAllAsync(spec)).FirstOrDefault();

            if (basket is null)
                return Result<BasketDto>.Failure(BasketError.NotFound(patientId));

            var item = basket.BasketItems
                .FirstOrDefault(i => i.MedicationId == medicationId);
            if (item is null)
                return Result<BasketDto>.Failure(MedicationError.NotFound(medicationId));

            item.Quantity = quantity;
            await _unitOfWork.SaveChangesAsync();
            var basketDto = _mapper.Map<BasketDto>(basket);
            return Result<BasketDto>.Success(basketDto);
        }

        public async Task<Result> RemoveItemAsync(int patientId, int medicationId)
        {
            var spec = new BasketByUserIdSpecs(patientId);
            var basket = (await _unitOfWork.GetRepository<CustomerBasket>().GetAllAsync(spec)).FirstOrDefault();

            if (basket is null)
                return Result.Failure(BasketError.NotFound(patientId));

            var item = basket.BasketItems
                .FirstOrDefault(i => i.MedicationId == medicationId);

            if (item is null)
                return Result<BasketDto>.Failure(MedicationError.NotFound(medicationId));

            _unitOfWork.GetRepository<BasketItem>().Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> ClearBasketAsync(int patientId)
        {
            var spec = new BasketByUserIdSpecs(patientId);
            var basket = (await _unitOfWork.GetRepository<CustomerBasket>().GetAllAsync(spec)).FirstOrDefault();

            if (basket is null)
                return Result.Failure(BasketError.NotFound(patientId)
                    );
            var itemRepo = _unitOfWork.GetRepository<BasketItem>();
            foreach(var item in basket.BasketItems.ToList())
            {
                itemRepo.Delete(item);
            }
            
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
    }
}