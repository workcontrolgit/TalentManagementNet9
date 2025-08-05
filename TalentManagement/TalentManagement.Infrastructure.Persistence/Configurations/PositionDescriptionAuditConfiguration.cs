using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Infrastructure.Persistence.Configurations
{
    public class PositionDescriptionAuditConfiguration : IEntityTypeConfiguration<PositionDescriptionAudit>
    {
        public void Configure(EntityTypeBuilder<PositionDescriptionAudit> builder)
        {
            builder.HasKey(e => e.PdAuditSeqNum);
            
            builder.Property(e => e.PdAuditSeqNum)
                .ValueGeneratedNever();

            builder.Property(e => e.PdNbr)
                .HasMaxLength(50);

            builder.Property(e => e.PdClassifierCommentsText)
                .HasMaxLength(2000);

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
        }
    }
}