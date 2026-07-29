using DAL.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations.UsersConfig
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {


            builder.Property(p => p.Address)
                   .HasMaxLength(250);

            builder.Property(p => p.DateOfBirth)
                   .IsRequired();

            builder.HasMany<Models.OrderModule.Order>()
                   .WithOne(o => o.Patient)
                   .HasForeignKey(o => o.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
