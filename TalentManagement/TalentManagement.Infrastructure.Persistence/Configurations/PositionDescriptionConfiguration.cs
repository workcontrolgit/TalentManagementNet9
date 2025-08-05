using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Infrastructure.Persistence.Configurations
{
    public class PositionDescriptionConfiguration : IEntityTypeConfiguration<PositionDescription>
    {
        public void Configure(EntityTypeBuilder<PositionDescription> builder)
        {
            builder.HasKey(e => e.PdSeqNum);
            
            builder.Property(e => e.PdSeqNum)
                .ValueGeneratedNever();

            builder.Property(e => e.PdNbr)
                .HasMaxLength(50);

            builder.Property(e => e.PdPositionTitleText)
                .HasMaxLength(200);

            builder.Property(e => e.PdOrgTitleText)
                .HasMaxLength(200);

            builder.Property(e => e.GvtOccSeries)
                .HasMaxLength(10);

            builder.Property(e => e.GvtPayPlan)
                .HasMaxLength(10);

            builder.Property(e => e.PdsStateCd)
                .HasMaxLength(10);

            builder.Property(e => e.GrdCode)
                .HasMaxLength(10);

            builder.Property(e => e.JobFunction)
                .HasMaxLength(50);

            // Configure audit relationship - this should work since we only kept this navigation property
            builder.HasMany(p => p.PositionDescriptionAudit)
                .WithOne(pa => pa.PdSeqNumNavigation)
                .HasForeignKey(pa => pa.PdSeqNum)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}