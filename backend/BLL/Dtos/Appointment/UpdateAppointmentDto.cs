using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Appointment
{
    public class UpdateAppointmentDto
    {
        public int? ScheduleId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? Notes { get; set; }
    }
}
