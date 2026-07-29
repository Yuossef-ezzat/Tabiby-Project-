using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.Admin
{
    public class UpdateUserRoleDto
    {
        [Required]
        public string Role { get; set; } = null!;

        public string? Specialty { get; set; }
        public string? Location { get; set; }
        public string? Specialization { get; set; }
        public string? PharmacyName { get; set; }
    }
}
