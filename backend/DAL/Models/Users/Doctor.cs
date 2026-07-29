using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Shared;

namespace DAL.Models.Users
{
    public class Doctor : ApplicationUser
    {
        public string Specialty { get; set; }

        public string Location { get; set; }

    }
}