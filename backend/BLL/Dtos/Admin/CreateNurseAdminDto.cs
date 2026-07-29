using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.Admin
{
    public class CreateNurseAdminDto
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

        [MaxLength(100)]
        public string? Specialization { get; set; }
    }
}
