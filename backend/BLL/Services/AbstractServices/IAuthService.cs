using BLL.Abstractions;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;
using Shared.DTOs.IdentityDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstractionLayer
{
    public interface IAuthService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
        Task<Result<UserDto>> RegisterAsync(RegisterDto RegisterDto);
        Task<Result> CheckEmailAsync (string email);
        Task<Result<UserDto>> GetCurrentUserAsync(string email);
        Task<Result<string>> GenerateResetTokenAsync(string email);
        Task<Result> SendResetEmailAsync(string email, string resetLink);
        Task<Result> ResetPasswordAsync(string email, string token, string newPassword);


    }
}
