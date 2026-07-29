using BLL.Abstractions;
using BLL.Dtos.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices.Admin
{
    public interface IAdminService
    {
        Task<Result<(IEnumerable<AdminUserDto> Users, int TotalCount)>> GetAllUsersAsync(SearchUserDto searchDto);
        Task<Result<AdminUserDto>> CreateDoctorAsync(CreateDoctorAdminDto dto);
        Task<Result<AdminUserDto>> CreateNurseAsync(CreateNurseAdminDto dto);
        Task<Result<AdminUserDto>> CreatePharmacistAsync(CreatePharmacistAdminDto dto);
        Task<Result<AdminUserDto>> DeleteUserAsync(int userId, int requestingAdminId);
        Task<Result<AdminUserDto>> ToggleUserActiveStatusAsync(int userId, int requestingAdminId);
        Task<Result<object>> GetSpecialtiesAsync();
    }
}
