using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Appointment
{
    public class CreateAppointmentDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string? Notes { get; set; }
    }
}
