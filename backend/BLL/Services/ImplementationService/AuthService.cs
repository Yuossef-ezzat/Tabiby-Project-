using AutoMapper;
using Microsoft.Extensions.Configuration;
using DAL.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using Shared.DTOs.IdentityDtos;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ServiceAbstractionLayer;
using PL.Utilites;
using BLL.Abstractions;
using BLL.Abstractions.Errors;
using BLL.Abstractions.Errrors;

namespace BLL.Services.ImplementationService
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
                            IConfiguration _configuration) : IAuthService
    {
        public async Task<Result> CheckEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure(UserError.UserNotFound(email));
            return Result.Success();
        }
        public async Task<Result<UserDto>> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<UserDto>.Failure(UserError.UserNotFound(email));
            var userDto = new UserDto()
            {
                Email = user.Email!,
                FullName = user.Fullname,
                Token = await GenerateJwtToken(user)
            };
            return Result<UserDto>.Success(userDto);
        }
        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var User = await _userManager.FindByEmailAsync(loginDto.Email);
            if (User is null)
                return Result<UserDto>.Failure(UserError.UserNotFound(loginDto.Email));

            var passwordValid = await _userManager.CheckPasswordAsync(User, loginDto.Password);

            if (!passwordValid)
                return Result<UserDto>.Failure(UserError.InvalidCredential());
            var UserDto = new UserDto
            {
                Email = User.Email!,
                FullName = User.Fullname,
                Token = await GenerateJwtToken(User)
            };
            return Result<UserDto>.Success(UserDto);

        }
        public async Task<Result<UserDto>> RegisterAsync(RegisterDto RegisterDto)
        {
            var user = new Patient
            {
                Fullname = RegisterDto.DisplayName,
                Email = RegisterDto.Email,
                UserName = RegisterDto.UserName ?? RegisterDto.Email.Split("@")[0],
                PhoneNumber = RegisterDto.PhoneNumber,
                UserType = "Patient",
            };
            
            var result = await _userManager.CreateAsync(user, RegisterDto.Password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "PATIENT");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return Result<UserDto>.Failure(new Errror($"{string.Join(", ", roleResult.Errors.Select(e => e.Description))}"));
                }
                var UserDto = new UserDto
                {
                    Email = user.Email!,
                    FullName = user.Fullname,
                    Token = await GenerateJwtToken(user)
                };
                return Result<UserDto>.Success(UserDto);
            }
            else
                return Result<UserDto>.Failure(new Errror($"{string.Join(", ", result.Errors.Select(e => e.Description))}"));
            
        }

        public async Task<Result<string>> GenerateResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<string>.Failure(UserError.UserNotFound(email));

            var ResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Result<string>.Success(ResetToken);
        }
        public async Task<Result> SendResetEmailAsync(string email, string resetLink)
        {
            var mail = new Email()
            {
                To = email,
                Subject = "Reset Password",
                Body = resetLink
            };
            var result = await EmailSettings.SendEmail(mail);
            if (result == false)
                return Result.Failure(UserError.FailedToSendEmail());
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure(UserError.UserNotFound(email));

            var decodedToken = Uri.UnescapeDataString(token);
            var res = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            if (!res.Succeeded)
                return Result.Failure(new Errror(string.Join(", ", res.Errors.Select(e=>e.Description))) );
            return Result.Success();
        }


        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.Name,user.UserName!),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()!)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var SecretKey = _configuration["JwtOptions:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey!));

            var Credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: Credentials
            );

            var TokenHandler = new JwtSecurityTokenHandler().WriteToken(Token);

            return TokenHandler;
        }


    }
}

