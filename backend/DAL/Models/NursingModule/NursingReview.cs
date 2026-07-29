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
    public class NursingReview : BaseEntity
    {

        public int NursingRequestId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        
        public virtual NursingRequest NursingRequest { get; set; }
    }
}
