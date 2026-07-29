using System.Collections.Generic;

namespace BLL.Dtos.Admin
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string UserType { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        
        public string? Specialty { get; set; }
        public string? Location { get; set; }
        public string? Specialization { get; set; }
        public string? PharmacyName { get; set; }
    }
}
