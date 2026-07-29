using DAL.Models.NursingModule;
using DAL.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Nursing
{
    public class NursingRequestDto
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public int NurseId { get; set; }
        public string NurseName { get; set; }

        public string CareType { get; set; }

        public string Status { get; set; } 

        
        public virtual NursingReviewDto Review { get; set; }
    }
}
