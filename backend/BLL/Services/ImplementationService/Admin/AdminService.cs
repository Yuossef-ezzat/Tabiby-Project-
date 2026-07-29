using AutoMapper;
using BLL.Dtos.Admin;
using BLL.Services.AbstractServices.Admin;
using DAL.Models.Users;
using Microsoft.AspNetCore.Identity;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Abstractions.Errors;

namespace BLL.Services.ImplementationService.Admin
{
    public class AdminService (UserManager<ApplicationUser> _userManager, IMapper _mapper, IUserRepository _userRepository) : IAdminService
    {
        public async Task<Result<(IEnumerable<AdminUserDto> Users, int TotalCount)>> GetAllUsersAsync(SearchUserDto searchDto)
        {
            var (users, totalCount) = await _userRepository.SearchUsersAsync(
                searchDto.Name,
                searchDto.Email,
                searchDto.UserType,
                searchDto.Role,
                searchDto.IsActive,
                searchDto.PageNumber,
                searchDto.PageSize);

            var mappedUsers = new List<AdminUserDto>();
            foreach (var user in users)
            {
                mappedUsers.Add(await MapToAdminUserDto(user));
            }

            return Result<(IEnumerable<AdminUserDto> Users, int TotalCount)>.Success((mappedUsers, totalCount));
        }

        public async Task<Result<AdminUserDto>> CreateDoctorAsync(CreateDoctorAdminDto dto)
        {
            var result = await EnsureEmailUniqueAsync(dto.Email);
            if (result.IsFailure)
                return Result<AdminUserDto>.Failure(result.Error);

            var doctor = new Doctor
            {
                Fullname = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.UserName ?? dto.Email.Split('@')[0],
                PhoneNumber = dto.PhoneNumber,
                UserType = "Doctor",
                IsActive = true,
                Specialty = dto.Specialty,
                Location = dto.Location ?? string.Empty
            };
            var createdDoctor = await CreateUserAndAssignRoleAsync(doctor, dto.Password, "DOCTOR");
            if (createdDoctor.IsFailure)
            {
                return Result<AdminUserDto>.Failure( createdDoctor.Error);
            }
            return Result<AdminUserDto>.Success(createdDoctor.Value);
        }

        public async Task<Result<AdminUserDto>> CreateNurseAsync(CreateNurseAdminDto dto)
        {
            var result = await EnsureEmailUniqueAsync(dto.Email);
            if (result.IsFailure)
                return Result<AdminUserDto>.Failure(result.Error);

            var nurse = new Nurse
            {
                Fullname = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.UserName ?? dto.Email.Split('@')[0],
                PhoneNumber = dto.PhoneNumber,
                UserType = "Nurse",
                IsActive = true,
                Specialization = dto.Specialization ?? string.Empty
            };
            var Nurse = await CreateUserAndAssignRoleAsync(nurse, dto.Password, "NURSE");
            if (Nurse.IsFailure) 
                return Result<AdminUserDto>.Failure( Nurse.Error);

            return Result<AdminUserDto>.Success(Nurse.Value);
        }

        public async Task<Result<AdminUserDto>> CreatePharmacistAsync(CreatePharmacistAdminDto dto)
        {
            var result = await EnsureEmailUniqueAsync(dto.Email);
            if (result.IsFailure)
                return Result<AdminUserDto>.Failure(result.Error);

            var pharmacist = new Pharmacist
            {
                Fullname = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.UserName ?? dto.Email.Split('@')[0],
                PhoneNumber = dto.PhoneNumber,
                UserType = "Pharmacist",
                IsActive = true,
                PharmacyName = dto.PharmacyName!
            };
            var createdPharmacist = await CreateUserAndAssignRoleAsync(pharmacist, dto.Password, "PHARNACIST");
            if (createdPharmacist.IsFailure) 
                return Result<AdminUserDto>.Failure(createdPharmacist.Error);
            return Result<AdminUserDto>.Success(createdPharmacist.Value);
        }

        public async Task<Result<AdminUserDto>> DeleteUserAsync(int userId, int requestingAdminId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result<AdminUserDto>.Failure(AdminError.NotFoundError(userId));

            if (userId == requestingAdminId)
                return Result<AdminUserDto>.Failure(AdminError.AdminCantBeDeleted());

            var isTargetAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isTargetAdmin)
                return Result<AdminUserDto>.Failure(AdminError.AdminCantBeDeleted());

            var mappedUser = await MapToAdminUserDto(user);
            
            try 
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<AdminUserDto>.Failure(new Errror(string.Join(", ", errors)));
                }
            }
            catch (DbUpdateException)
            {
                return Result<AdminUserDto>.Failure(AdminError.AdminCantBeDeleted());
            }

            return Result<AdminUserDto>.Success(mappedUser);
        }



        public async Task<Result<object>> GetSpecialtiesAsync()
        {
            var specialties = await _userRepository.GetDistinctSpecialtiesAsync();
            return Result<object>.Success(specialties);
        }

        public async Task<Result<AdminUserDto>> ToggleUserActiveStatusAsync(int userId, int requestingAdminId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result<AdminUserDto>.Failure(AdminError.NotFoundError(userId));

            if (userId == requestingAdminId)
                return Result<AdminUserDto>.Failure(AdminError.AdminCantBeDisActive());

            var isTargetAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isTargetAdmin)
                return Result<AdminUserDto>.Failure(AdminError.AdminCantBeDisActive());
            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<AdminUserDto>.Failure(new Errror(string.Join(", ", errors)));

            }
            var mappedUser = await MapToAdminUserDto(user);
            return Result<AdminUserDto>.Success(mappedUser);
        }

        
        private async Task<Result> EnsureEmailUniqueAsync(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
                return Result.Failure(AdminError.UserAlreadyExists(email));

            return Result.Success();
        }

        private async Task<Result<AdminUserDto>> CreateUserAndAssignRoleAsync(ApplicationUser user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<AdminUserDto>.Failure(new Errror("Identity.CreateFailed", string.Join(", ", errors)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(e => e.Description).ToList();
                return Result<AdminUserDto>.Failure(new Errror("Identity.CreateFailed", string.Join(", ", errors)));
            }
            var mappedUser =await MapToAdminUserDto(user);
            return Result<AdminUserDto>.Success(mappedUser);
        }

        private async Task<AdminUserDto> MapToAdminUserDto(ApplicationUser user)
        {
            var mappedUser = _mapper.Map<AdminUserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            mappedUser.Roles = roles.ToList();

            if (user is Doctor doc)
            {
                mappedUser.Specialty = doc.Specialty;
                mappedUser.Location = doc.Location;
            }
            else if (user is Nurse nurse)
            {
                mappedUser.Specialization = nurse.Specialization;
            }
            else if (user is Pharmacist pharm)
            {
                mappedUser.PharmacyName = pharm.PharmacyName;
            }

            return mappedUser;
        }
    }
}
