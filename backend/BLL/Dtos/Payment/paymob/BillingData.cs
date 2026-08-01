using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Payment.paymob
{
    public class BillingData
    {
        public string FullName { get; set; } = "N/A";
        public string Email { get; set; } 
        public string PhoneNumber { get; set; }
        public string Apartment { get; set; } = "NA";
        public string Floor { get; set; } = "NA";
        public string Building { get; set; } = "NA";
        public string Street { get; set; } = "NA";
        public string City { get; set; } = "NA";
        public string Country { get; set; } = "EG";
    }
}
