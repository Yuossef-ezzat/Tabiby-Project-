using DAL.Models.AppointmentModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {

            builder.Property(ds => ds.DayOfWeek)
                   .IsRequired();

            builder.Property(ds => ds.StartTime)
                   .IsRequired();

            builder.Property(ds => ds.EndTime)
                   .IsRequired();

            builder.Property(ds => ds.IsAvailable)
                   .HasDefaultValue(true);

            builder.HasOne<DAL.Models.Users.Doctor>()
                   .WithMany()
                   .HasForeignKey(ds => ds.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ds => ds.Appointments)
                   .WithOne(a => a.Schedule)
                   .HasForeignKey(a => a.ScheduleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
