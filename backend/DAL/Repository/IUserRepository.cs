using DAL.Models.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IUserRepository
    {
        Task<Patient?> GetPatientWithBasketAsync(int patientId);
        Task<Patient?> GetPatientWithAppointmentAsync(int patientId);
        Task<Pharmacist?> GetPharmacistWithMedicationsAsync(int pharmacistId);
        Task<Doctor?> GetDoctorByIdAsync(int doctorId);
        Task<IEnumerable<Doctor>> SearchDoctorsAsync(string? name, string? specialization, string? location, int pageNumber, int pageSize);
        Task<IEnumerable<Nurse>> SearchNursesAsync(string? name, string? specialization, int pageNumber, int pageSize);
        Task<(IEnumerable<ApplicationUser> Users, int TotalCount)> SearchUsersAsync(string? name, string? email, string? userType, string? role, bool? isActive, int pageNumber, int pageSize);
        Task<bool> HasActiveAppointmentsAsync(int doctorId);
        Task<bool> HasActiveConsultationsAsync(int doctorId);
        Task<bool> HasActiveNursingRequestsAsync(int nurseId);
        Task<bool> HasMedicationsAsync(int pharmacistId);
        Task ConvertUserTypeAsync(int userId, string targetType, string? specialty, string? location, string? specialization, string? pharmacyName);
        Task<object> GetDistinctSpecialtiesAsync();
    }
}
