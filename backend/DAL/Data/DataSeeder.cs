using DAL.Models;
using DAL.Models.AppointmentModule;
using DAL.Models.OrderModule;
using DAL.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace DAL.Data
{
    public class DataSeeder
    {
        private readonly TabibyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private const string SEED_DATA_FOLDER = "DataSeeding";
        private readonly string _seedPath;

        public DataSeeder(
            TabibyDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _seedPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "DAL", "Data");
        }
        public async Task SeedDatabaseAsync()
        {
            try
            {
                Console.WriteLine("Starting database seeding process...");

                // 1. Seed Roles (يتم تشغيلها دائماً للتأكد من وجود الأدوار حتى لو كانت قاعدة البيانات تحتوي على مستخدمين)
                await SeedRolesAsync();
                Console.WriteLine("✓ Roles checked/seeded successfully");

                

                
                await SeedUsersAsync();
                Console.WriteLine("✓ Users seeded successfully");

                
                await SeedMedicationsAsync();
                Console.WriteLine("✓ Medications seeded successfully");

                
                await SeedDoctorSchedulesAsync();
                Console.WriteLine("✓ Doctor Schedules seeded successfully");

                Console.WriteLine("✓ Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error during seeding: {ex.Message}");
                throw;
            }
        }

        private async Task SeedUsersAsync()
        {
            var path = Path.Combine(_seedPath, SEED_DATA_FOLDER, "users-seed.json");
            if (!File.Exists(path)) { Console.WriteLine("✗ Users file not found"); return; }

            var json = await File.ReadAllTextAsync(path);
            var seedData = JsonSerializer.Deserialize<List<UserSeedDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (seedData is null) return;

            foreach (var dto in seedData)
            {
                var existing = await _userManager.FindByEmailAsync(dto.Email);
                if (existing != null) continue;

                ApplicationUser user = dto.UserType switch
                {
                    "Doctor" => new Doctor
                    {
                        Specialty = dto.Specialty ?? string.Empty,
                        Location = dto.Location ?? string.Empty
                    },
                    "Patient" => new Patient
                    {
                        Address = dto.Address ?? string.Empty,
                        DateOfBirth = dto.DateOfBirth ?? DateTime.UtcNow
                    },
                    "Nurse" => new Nurse
                    {
                        Specialization = dto.Specialization ?? string.Empty
                    },
                    "Pharmacist" => new Pharmacist
                    {
                        PharmacyName = dto.PharmacyName ?? string.Empty
                    },
                    _ => new ApplicationUser()
                };

                user.Id = dto.Id;
                user.UserName = dto.UserName;
                user.UserType = dto.UserType;
                user.Email = dto.Email;
                user.Fullname = dto.Fullname;
                user.IsActive = dto.IsActive;

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    Console.WriteLine($" Failed to create {dto.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(dto.Role) && await _roleManager.RoleExistsAsync(dto.Role))
                    await _userManager.AddToRoleAsync(user, dto.Role);
            }
        }

        private async Task SeedRolesAsync()
        {
            
            
            
            
            
            string[] defaultRoles = { "PATIENT", "DOCTOR", "ADMIN", "NURSE", "PHARMACIST" };
            foreach (var roleName in defaultRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int> 
                    { 
                        Name = roleName, 
                        NormalizedName = roleName.ToUpper() 
                    });
                }
            }

            
            var RolesSeedPath = Path.Combine(_seedPath, SEED_DATA_FOLDER, "Roles.json");
            if (!File.Exists(RolesSeedPath))
            {
                Console.WriteLine("Roles seed file not found, using default hardcoded roles.");
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(RolesSeedPath);
                var roles = JsonSerializer.Deserialize<List<IdentityRole<int>>>(json);

                if (roles is not null)
                {
                    foreach (var role in roles)
                    {
                        if (!string.IsNullOrEmpty(role.Name) && !await _roleManager.RoleExistsAsync(role.Name))
                        {
                            await _roleManager.CreateAsync(role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to parse Roles.json, error: {ex.Message}");
            }
        }

        private async Task SeedMedicationsAsync()
        {
            var path = Path.Combine(_seedPath, SEED_DATA_FOLDER, "medications-seed.json");
            if (!File.Exists(path)) return;

            var json = await File.ReadAllTextAsync(path);
            var seedData = JsonSerializer.Deserialize<List<Medication>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (seedData is null) return;

            foreach (var item in seedData)
            {
                if (await _context.Medications.AnyAsync(m => m.Id == item.Id)) continue;
                _context.Medications.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedDoctorSchedulesAsync()
        {
            var path = Path.Combine(_seedPath, SEED_DATA_FOLDER, "doctorSchedules-seed.json");
            if (!File.Exists(path)) return;

            var json = await File.ReadAllTextAsync(path);
            var seedData = JsonSerializer.Deserialize<List<DoctorSchedule>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (seedData is null) return;

            foreach (var item in seedData)
            {
                if (await _context.DoctorSchedules.AnyAsync(s => s.Id == item.Id)) continue;
                _context.DoctorSchedules.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        public class UserSeedDto
        {
            public int Id { get; set; }
            public string UserType { get; set; } = null!;  
            public string Fullname { get; set; } = null!;
            public string UserName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
            public bool IsActive { get; set; }
            public string Role { get; set; } = null!;

            
            public string? Specialty { get; set; }
            public string? Location { get; set; }

            
            public string? Address { get; set; }
            public DateTime? DateOfBirth { get; set; }

            
            public string? Specialization { get; set; }

            
            public string? PharmacyName { get; set; }
        }
    }
}