using BLL.Abstractions;
using BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices
{
    public interface IProfileUserService
    {
        Task<Result<ProfileUser>> GetProfileUserByIdAsync(int id);
        Task<Result<ProfileUser>> UpdateMyProfile(int id, ProfileUser profile);
    }
}
