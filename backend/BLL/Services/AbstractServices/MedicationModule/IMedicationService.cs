using BLL.Abstractions;
using BLL.Dtos.Medication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.MedicationModule
{
    public interface IMedicationService
    {
       Task<Result<MedicationDto>> GetMedicationByIdAsync(int id);
       Task<Result<IEnumerable<AllMedicationDto>>> GetAllMedicationsAsync(string? SearchName);
       Task<Result> UpdateMedicationAsync(int PharmacistId,MedicationDto medicationDto);
       Task<Result<MedicationDto>> CreateMedicationAsync(int PharmacistId,CreateMedicationDto medicationDto);
       Task<Result> DeleteMedicationAsync(int PharmacistId,int id);


    }
}
