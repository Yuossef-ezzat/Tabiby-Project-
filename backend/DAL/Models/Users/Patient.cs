using DAL.Models.AppointmentModule;
using DAL.Shared;
using DomainLayer.Models.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Users
{
    public class Patient : ApplicationUser
    {
        public string? Address { get; set; }

        public DateTime DateOfBirth { get; set; }
        public CustomerBasket? Basket { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
