using BLL.Abstractions;
using BLL.Dtos.Consultion;
using BLL.Dtos.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.ConsultationModule
{
    public interface IConsultationService
    {
            Task<Result<DoctorInfoDto>> GetDoctorInfoAsync(int doctorId);
            Task<Result<IEnumerable<DoctorInfoDto>>> SearchDoctorsAsync(SearchDoctorDto searchDto);
            Task<Result<ConsultationDto>> GetConsultationByIdAsync(int ConsultationId , int RequesterId);
            Task<Result<IEnumerable<ConsultationDto>>> GetMyConsultationsAsync(int PatientId);
            Task<Result<ConsultationDto>> RequestConsultationAsync(int PatientId, CreateConsultationDto createDto);
            Task<Result<ConsultationDto>> UpdateConsultationStatusAsync(int consultationId, int PatientId, UpdateConsultionStatusDto updateStatusDto);
            Task<Result> DeleteConsultationAsync(int ConsultationId,int RequesterId);
    }
}
