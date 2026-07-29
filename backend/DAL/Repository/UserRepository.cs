using DAL.Data;
using DAL.Models.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UserRepository(TabibyDbContext _context) : IUserRepository
    {

        public async Task<Patient?> GetPatientWithAppointmentAsync(int patientId)
        {
            return await _context
                            .Set<Patient>()
                            .Include(p => p.Appointments)
                            .FirstOrDefaultAsync(p => p.Id == patientId);
        }        

        public async Task<Patient?> GetPatientWithBasketAsync(int patientId)
        {
            return await _context
                            .Set<Patient>()
                            .Include(p => p.Basket)
                            .FirstOrDefaultAsync(p => p.Id == patientId);
        }
        public async Task<Pharmacist?> GetPharmacistWithMedicationsAsync(int pharmacistId)
        {
            return await _context
                            .Set<Pharmacist>()
                            .Include(p => p.Medications)
                            .FirstOrDefaultAsync(p => p.Id == pharmacistId);
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int doctorId)
        {
            return await _context
                            .Set<Doctor>()
                            .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<IEnumerable<Doctor>> SearchDoctorsAsync(string? name, string? specialization, string? location, int pageNumber, int pageSize)
        {
            var query = _context.Set<Doctor>().Where(d => d.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.Fullname.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrWhiteSpace(specialization))
                query = query.Where(d => d.Specialty.ToLower().Contains(specialization.ToLower()));

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(d => d.Location.ToLower().Contains(location.ToLower()));

            return await query
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<IEnumerable<Nurse>> SearchNursesAsync(string? name, string? specialization, int pageNumber, int pageSize)
        {
            var query = _context.Set<Nurse>().Where(n => n.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(n => n.Fullname.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrWhiteSpace(specialization))
                query = query.Where(n => n.Specialization.ToLower().Contains(specialization.ToLower()));

            return await query
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<(IEnumerable<ApplicationUser> Users, int TotalCount)> SearchUsersAsync(string? name, string? email, string? userType, string? role, bool? isActive, int pageNumber, int pageSize)
        {
            var query = _context.Set<ApplicationUser>().AsQueryable();

            
            query = query.Where(u => u.UserType != "Admin");

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(u => u.Fullname.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.ToLower().Contains(email.ToLower()));

            if (!string.IsNullOrWhiteSpace(userType))
                query = query.Where(u => u.UserType.ToLower() == userType.ToLower());

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleId = await _context.Roles
                    .Where(r => r.Name.ToLower() == role.ToLower())
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (roleId != 0)
                {
                    var userIds = _context.UserRoles.Where(ur => ur.RoleId == roleId).Select(ur => ur.UserId);
                    query = query.Where(u => userIds.Contains(u.Id));
                }
                else
                {
                    query = query.Where(u => false);
                }
            }

            int totalCount = await query.CountAsync();

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (users, totalCount--);
        }

        public async Task<bool> HasActiveAppointmentsAsync(int doctorId)
        {
            return await _context.Appointments.AnyAsync(a => a.DoctorId == doctorId && (a.Status == DAL.Shared.Enums.AppointmentStatus.Pending || a.Status == DAL.Shared.Enums.AppointmentStatus.Confirmed));
        }

        public async Task<bool> HasActiveConsultationsAsync(int doctorId)
        {
            return await _context.Consultations.AnyAsync(c => c.DoctorId == doctorId && c.Status == DAL.Shared.Enums.ConsultationStatus.Pending);
        }

        public async Task<bool> HasActiveNursingRequestsAsync(int nurseId)
        {
            return await _context.NursingRequests.AnyAsync(n => n.NurseId == nurseId && n.Status == "Pending");
        }

        public async Task<bool> HasMedicationsAsync(int pharmacistId)
        {
            return await _context.Medications.AnyAsync(m => m.PharmacistId == pharmacistId);
        }

        public async Task ConvertUserTypeAsync(int userId, string targetType, string? specialty, string? location, string? specialization, string? pharmacyName)
        {
            var sql = @"
                UPDATE AspNetUsers
                SET Discriminator = {0},
                    UserType = {0},
                    Specialty = {1},
                    Location = {2},
                    Specialization = {3},
                    PharmacyName = {4}
                WHERE Id = {5}";
            
            await _context.Database.ExecuteSqlRawAsync(sql, 
                targetType,
                targetType == "Doctor" ? (object?)specialty ?? DBNull.Value : DBNull.Value,
                targetType == "Doctor" ? (object?)location ?? DBNull.Value : DBNull.Value,
                targetType == "Nurse" ? (object?)specialization ?? DBNull.Value : DBNull.Value,
                targetType == "Pharmacist" ? (object?)pharmacyName ?? DBNull.Value : DBNull.Value,
                userId);
        }
        public async Task<object> GetDistinctSpecialtiesAsync()
        {
            var doctors = await _context.Set<Doctor>()
                .Where(d => !string.IsNullOrEmpty(d.Specialty))
                .Select(d => d.Specialty)
                .Distinct()
                .ToListAsync();

            var nurses = await _context.Set<Nurse>()
                .Where(n => !string.IsNullOrEmpty(n.Specialization))
                .Select(n => n.Specialization)
                .Distinct()
                .ToListAsync();

            return new { doctors, nurses };
        }
    }
}
