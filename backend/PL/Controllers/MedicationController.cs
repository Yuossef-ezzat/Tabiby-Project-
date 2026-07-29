using BLL.Dtos.Medication;
using BLL.Services.AbstractServices.MedicationModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PL.Models.MedicationModels;
using PresentationLayer.Controller;

namespace PL.Controllers
{
    public class MedicationController(IMedicationService _medicationService) : ApiControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllMedications(string? SearchName)
        {
            var result = await _medicationService.GetAllMedicationsAsync(SearchName);

            return Ok(result.Value);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetMedicationById(int id)
        {
            var result = await _medicationService.GetMedicationByIdAsync(id);
            if (result.IsFailure)
                return NotFound(result.Error);
            return Ok(result);
        }

        #region Pharmcist - Functionality 
        [Authorize(Roles = "PHARMACIST,ADMIN")]
        [HttpPost("UpdateMedication/{Id}")]
        public async Task<IActionResult> UpdateMedication(int Id, UpdateMedicationModel model)
        {
            var result = await _medicationService.GetMedicationByIdAsync(Id);
            if (result.IsFailure)
                return NotFound(result.Error);
            result.Value.Id = Id;
            result.Value.Name = model.Name;
            result.Value.Price = model.Price;
            result.Value.Stock = model.Stock;
            result.Value.IsAvailable = model.IsAvailable;
            var result1 = await _medicationService.UpdateMedicationAsync(User.GetUserId(), result.Value);
            if (result1.IsFailure)
                return BadRequest(result1.Error);
            return Ok(result.Value);
        }
        [Authorize(Roles = "PHARMACIST,ADMIN")]
        [HttpPost("CreateMedication")]
        public async Task<IActionResult> CreateMedication([FromForm] CreateMedicationDto dto)
        {
            var result = await _medicationService.CreateMedicationAsync(User.GetUserId(), dto);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }
        [Authorize(Roles = "PHARMACIST,ADMIN")]
        [HttpDelete("DeleteMedication/{id}")]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var result = await _medicationService.DeleteMedicationAsync(User.GetUserId(), id);
            if (result.IsFailure)
                return NotFound(result.Error);
            return NoContent();
        } 
        #endregion
    }
}
