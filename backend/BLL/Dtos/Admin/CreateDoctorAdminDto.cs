using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.Admin
{
    public class CreateDoctorAdminDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string DisplayName { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? UserName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; } = null!;

        [MaxLength(250)]
        public string? Location { get; set; }
    }
}
