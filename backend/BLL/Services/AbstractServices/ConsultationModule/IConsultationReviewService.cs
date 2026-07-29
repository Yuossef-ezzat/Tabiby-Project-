using BLL.Abstractions;
using BLL.Dtos.Consultion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.ConsultationModule
{
    public interface IConsultationReviewService
    {
            Task<Result<ConsultationReviewDto>> AddReviewAsync(int consultationId, int patientId, CreateConsultationReviewDto dto);
            Task<Result<ConsultationReviewDto?>> GetConsultationReviewsByConsultationId(int consultationId);
    }
}
