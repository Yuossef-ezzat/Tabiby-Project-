using AutoMapper;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Medication;
using BLL.Services.AbstractServices;
using BLL.Services.AbstractServices.MedicationModule;
using DAL.Models.AppointmentModule;
using DAL.Models.OrderModule;
using DAL.Models.Users;
using DAL.Repository;
using DAL.Specifications.OrderSpecs;


namespace BLL.Services.ImplementationService.MedicationModule
{
    public class MedicationService : IMedicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenaricRepository<Medication> _repo;
        private readonly IAttachmentService _attach;
        private readonly IUserRepository _userRepository;

        public MedicationService(IUnitOfWork unitOfWork, IMapper mapper , IAttachmentService attach, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = _unitOfWork.GetRepository<Medication>();
            _attach = attach;
            _userRepository = userRepository;
        }

        public async Task<Result<MedicationDto>> CreateMedicationAsync(int PharmacistId,CreateMedicationDto medicationDto)
        {
            var isPharmacist = await ValidatePharmacist(PharmacistId);
            if (isPharmacist.IsFailure)
                return Result<MedicationDto>.Failure(isPharmacist.Error);

            var medicationEntity = _mapper.Map<Medication>(medicationDto);

            medicationEntity.PharmacistId = PharmacistId;
            if (medicationDto.Image != null)
            {
                var imageUrl = await _attach.Upload(medicationDto.Image, "medications");
                medicationEntity.PictureUrl = imageUrl;
            }
            await _repo.AddAsync(medicationEntity);
            await _unitOfWork.SaveChangesAsync();
            var medicationDtoToReturn = _mapper.Map<MedicationDto>(medicationEntity);
            return Result<MedicationDto>.Success(medicationDtoToReturn);
        }

        public async Task<Result> DeleteMedicationAsync(int PharmacistId, int id)
        {
            var isPharmacist = await ValidatePharmacist(PharmacistId);
            if (isPharmacist.IsFailure)
                return Result.Failure(isPharmacist.Error);

            var medicationEntity = await _repo.GetByIdAsync(id);
            if (medicationEntity is null)
                return Result.Failure(MedicationError.NotFound(id));

            _attach.Delete(medicationEntity?.PictureUrl!);
            _repo.Delete(medicationEntity!);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<IEnumerable<AllMedicationDto>>> GetAllMedicationsAsync(string? SearchName)
        {
            var medications = await _repo.GetAllAsync(new AllMedicationOrderedSpecs(SearchName));
            if (!medications?.Any() ?? true)
                return Result<IEnumerable<AllMedicationDto>>.Success(Enumerable.Empty<AllMedicationDto>());
            
            return Result< IEnumerable < AllMedicationDto >>.Success(_mapper.Map<IEnumerable<AllMedicationDto>>(medications));
        }

        public async Task<Result<MedicationDto>> GetMedicationByIdAsync(int id)
        {
            var medicationEntity = await _repo.GetByIdAsync(id);
            if (medicationEntity is null)
                return Result<MedicationDto>.Failure(MedicationError.NotFound(id));

            var medicationDto = _mapper.Map<MedicationDto>(medicationEntity);
            return Result<MedicationDto>.Success(medicationDto);
        }
        public async Task<Result> UpdateMedicationAsync(int PharmacistId,MedicationDto medicationDto)
        {
            var isPharmacist = await ValidatePharmacist(PharmacistId);
            if (isPharmacist.IsFailure)
                return Result.Failure(isPharmacist.Error);

            var medicationEntity = await _repo.GetByIdAsync(medicationDto.Id);
            if (medicationEntity is null)
                return Result<MedicationDto>.Failure(MedicationError.NotFound(medicationDto.Id));

            medicationEntity.Name = medicationDto.Name;
            medicationEntity.Price = medicationDto.Price;
            medicationEntity.Stock = medicationDto.Stock;
            medicationEntity.IsAvailable = medicationDto.IsAvailable;
             _repo.Update(medicationEntity);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        private async Task<Result> ValidatePharmacist(int Id)
        {
            var pharmacist = await _userRepository.GetPharmacistWithMedicationsAsync(Id);
            if (pharmacist == null)
                return Result<bool>.Failure(PharmacistError.NotFound(Id));

            if (pharmacist.UserType == "Pharmacist")
                return Result.Success();
            return Result.Failure(PharmacistError.UnAuthorizedAccess());
        }


    }
}
