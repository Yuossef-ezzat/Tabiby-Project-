using DAL.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations.UsersConfig
{
    public class NurseConfiguration : IEntityTypeConfiguration<Nurse>
    {
        public void Configure(EntityTypeBuilder<Nurse> builder)
        {


            builder.Property(n => n.Specialization)
                   .HasMaxLength(100);

            builder.HasMany(n => n.NursingRequests)
                   .WithOne(nr => nr.Nurse)
                   .HasForeignKey(nr => nr.NurseId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
