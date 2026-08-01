using DAL.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.OrderModule
{
    public class OrderAddress 
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Apartment { get; set; } = "NA";
        public string Building { get; set; } = "NA";
        public string Floor { get; set; } = "NA";
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Street { get; set; } = null!;
    }
}
