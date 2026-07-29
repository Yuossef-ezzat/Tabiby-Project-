using DAL.Models.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations.OrderConfig
{
    public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {

            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.Price)
                   .HasColumnType("decimal(10,2)");

            builder.Property(m => m.Stock)
                   .IsRequired();

            builder.Property(m => m.IsAvailable)
                   .HasDefaultValue(true);

            
            
            builder.HasMany(m => m.Order_Item)
                   .WithOne(oi => oi.Medication)
                   .HasForeignKey(oi => oi.MedicationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
