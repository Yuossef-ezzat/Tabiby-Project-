using BLL.Abstractions;
using BLL.Abstractions.Errrors;
using BLL.Dtos;
using BLL.Services.AbstractServices;
using DAL.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BLL.Services.ImplementationService
{
    public class ProfileUserService(UserManager<ApplicationUser> userManager ) : IProfileUserService
    {
        public async Task<Result<ProfileUser>> GetProfileUserByIdAsync(int id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) 
                return Result<ProfileUser>.Failure(UserError.UserNotFound(id));

            var profile = new ProfileUser
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty
            };

            if (user is Doctor doctor)
            {
                profile.Specialty = doctor.Specialty ?? string.Empty;
                profile.Location = doctor.Location ?? string.Empty;
            }
            else if (user is Patient patient)
            {
                profile.Address = patient.Address ?? string.Empty;
                profile.DateOfBirth = patient.DateOfBirth;
            }
            else if (user is Nurse nurse)
            {
                profile.Specialization = nurse.Specialization ?? string.Empty;
            }
            else if (user is Pharmacist pharmacist)
            {
                profile.PharmacyName = pharmacist.PharmacyName ?? string.Empty;
            }

            return Result<ProfileUser>.Success(profile);
        }

        public async Task<Result<ProfileUser>> UpdateMyProfile(int id, ProfileUser profile)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) 
                return Result<ProfileUser>.Failure(UserError.UserNotFound(id));

            user.Fullname = profile.Fullname;
            user.Email = profile.Email;
            user.UserName = profile.Email.Substring(0, profile.Email.IndexOf('@'));
            user.PhoneNumber = profile.Phone;

            if (user is Doctor doctor)
            {
                doctor.Specialty = profile.Specialty!;
                doctor.Location = profile.Location!;
            }
            else if (user is Patient patient)
            {
                patient.Address = profile.Address;
                patient.DateOfBirth = profile!.DateOfBirth!.Value;
            }
            else if (user is Nurse nurse)
            {
                nurse.Specialization = profile.Specialization!;
            }
            else if (user is Pharmacist pharmacist)
            {
                pharmacist.PharmacyName = profile.PharmacyName!;
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<ProfileUser>.Failure(UserError.ProfileUpdateFailed(errors));
            }

            return Result<ProfileUser>.Success(profile);
        }
    }
}
