using DAL.Models.Consultation;
using DAL.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations.ConsultionConfig
{
    public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
    {
        public void Configure(EntityTypeBuilder<Consultation> builder)
        {
            builder.Property(c => c.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(ConsultationStatus.Pending);

            builder.Property(c => c.RequestedAt)
                   .IsRequired();

            builder.Property(c => c.CreatedAt)
                   .ValueGeneratedOnAdd();



            builder.HasOne(c => c.Patient)
                   .WithMany()
                   .HasForeignKey(c => c.PatientId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(c => c.Doctor)
                   .WithMany()
                   .HasForeignKey(c => c.DoctorId)
                   .OnDelete(DeleteBehavior.NoAction);  

            builder.HasOne(c => c.Review)
                   .WithOne(r => r.Consultation)
                   .HasForeignKey<ConsultationReview>(r => r.ConsultationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
