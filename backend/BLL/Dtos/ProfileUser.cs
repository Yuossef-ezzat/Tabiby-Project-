using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos
{
    public class ProfileUser
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }
        public string? Specialty { get; set; } = null!;
        public string? Location { get; set; } = null!;
        public string? PharmacyName { get; set; } = null!;
        public string? Specialization { get; set; } = null!;


    }
}
