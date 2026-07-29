using DAL.Models.Users;
using DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.NursingModule
{
    public class NursingRequest : BaseEntity
    {

        public int PatientId { get; set; }

        public int NurseId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RequestedDate { get; set; }

        public string CareType { get; set; }

        public string Status { get; set; } = "Pending";


        
        public virtual Patient Patient { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual NursingReview Review { get; set; }
    }
}
