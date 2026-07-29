using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data.Configurations.ConsultionConfig
{
    public class ConsultationMessageConfiguration : IEntityTypeConfiguration<ConsultationMessage>
    {
        public void Configure(EntityTypeBuilder<ConsultationMessage> builder)
        {

            builder.Property(cm => cm.CreatedAt)
                   .ValueGeneratedOnAdd();

            builder.HasOne(cm => cm.Consultation)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(cm => cm.ConsultationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cm => cm.Sender)
                   .WithMany()
                   .HasForeignKey(cm => cm.SenderUserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}