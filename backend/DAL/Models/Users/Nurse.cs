using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;
using DAL.Models.NursingModule;
using DAL.Shared;

namespace DAL.Models.Users
{
    public class Nurse : ApplicationUser
    {

        public string Specialization { get; set; } = string.Empty;

        public virtual ICollection<NursingRequest> NursingRequests { get; set; } = new List<NursingRequest>();
        
    }
}
