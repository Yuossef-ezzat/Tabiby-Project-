using DAL.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations.UsersConfig
{
    public class PharmacistConfiguration : IEntityTypeConfiguration<Pharmacist>
    {
        public void Configure(EntityTypeBuilder<Pharmacist> builder)
        {



            builder.Property(p => p.PharmacyName)
                   .HasMaxLength(200);

            builder.HasMany(p => p.Medications)
                   .WithOne(m => m.Pharmacist)
                   .HasForeignKey(m => m.PharmacistId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
