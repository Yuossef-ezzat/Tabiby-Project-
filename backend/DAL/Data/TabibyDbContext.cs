using DAL.Models;
using DAL.Models.AppointmentModule;
using DAL.Models.Consultation;
using DAL.Models.NursingModule;
using DAL.Models.OrderModule;
using DAL.Models.Users;
using DomainLayer.Models.BasketModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class TabibyDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {


        public TabibyDbContext(DbContextOptions<TabibyDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TabibyDbContext).Assembly);
        }

        
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }

        
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<CustomerBasket> CustomerBaskets { get; set; }


        
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<ConsultationMessage> ConsultationMessages { get; set; }

        public DbSet<ConsultationReview> ConsultationReviews { get; set; }

        
        public DbSet<NursingRequest> NursingRequests { get; set; }
        public DbSet<NursingReview> NursingReviews { get; set; }

        
        public DbSet<Notification> Notifications { get; set; }

    }
}
