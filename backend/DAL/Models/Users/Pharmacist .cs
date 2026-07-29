
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;
using DAL.Models.OrderModule;
using DAL.Shared;


namespace DAL.Models.Users
{
    public class Pharmacist : ApplicationUser
    {
        public string PharmacyName { get; set; } = string.Empty;

        public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
    }
}