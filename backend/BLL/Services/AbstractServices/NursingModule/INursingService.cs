using BLL.Abstractions;
using BLL.Dtos.Nursing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.Users
{
    public interface INursingService
    {
        Task<Result<NursingRequestDto>> RequestNursingAsync(int patientId, CreateNursingRequestDto dto);
        Task<Result<IEnumerable<NursingRequestDto>>> GetMyNursingRequestsAsync(int requesterId); 
        Task<Result<NursingRequestDto>> UpdateNursingStatusAsync(int requestId, UpdateNursingStatusDto dto, int userId); 
        Task<Result<NursingRequestDto>> CancelNursingAsync(int requestId, int userId); 
        Task<Result<NursingReviewDto>> AddNursingReviewAsync(int requestId, int patientId, CreateNursingReviewDto dto); 
        Task<Result<IEnumerable<NursingReviewDto>>> GetNursingReviewAsync(int requestId); 
        Task<Result<IEnumerable<NurseInfoDto>>> SearchNursesAsync(SearchNurseDto searchDto);

    }
}
