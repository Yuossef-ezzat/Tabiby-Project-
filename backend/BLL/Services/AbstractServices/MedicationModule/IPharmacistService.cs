using BLL.Dtos.Medication;


namespace BLL.Services.AbstractServices.MedicationModule
{
    public interface IPharmacistService
    {
        Task<int> CreateMedicationAsync(CreateMedicationDto medication);
        Task<int> UpdateMedicationAsync(UpdateMedicationDto medication);
        Task<int> DeleteMedicationAsync(int id);
    }
}
