using DAL.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations.UsersConfig
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {


            builder.Property(d => d.Specialty)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Location)
                   .HasMaxLength(250);


        }
    }
}
