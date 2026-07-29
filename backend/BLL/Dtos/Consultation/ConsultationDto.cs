using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Consultion
{
    public class ConsultationDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; } 
        public string PatientName { get; set; } = null!;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime RequestedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public ConsultationReviewDto? Review { get; set; }
    }
}
