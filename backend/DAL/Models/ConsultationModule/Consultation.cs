using DAL.Models.Users;
using DAL.Shared;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Consultation
{
    
    public class Consultation : BaseEntity
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public ConsultationStatus Status { get; set; } 

        public DateTime RequestedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        
        public virtual ICollection<ConsultationMessage> Messages { get; set; } = new List<ConsultationMessage>();

        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual ConsultationReview? Review { get; set; }
    }
}
