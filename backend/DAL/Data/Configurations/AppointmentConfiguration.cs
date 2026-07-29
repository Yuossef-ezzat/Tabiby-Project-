using DAL.Models.AppointmentModule;
using DAL.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {

            builder.Property(a => a.AppointmentDate)
                   .IsRequired();

            builder.Property(a => a.AppointmentTime)
                   .IsRequired();

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(AppointmentStatus.Pending);

            builder.Property(a => a.Notes)
                   .HasMaxLength(500);

            builder.HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Doctor)
                   .WithMany()
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.NoAction);  

        }
    }
}
