using Domain.Core.Entities;
using Domain.Core.Enums;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class TransactionImportRowConfig : IEntityTypeConfiguration<TransactionImportRow>
{
    public void Configure(EntityTypeBuilder<TransactionImportRow> entity)
    {
        entity.ToTable(nameof(TransactionImportRow), SchemaConstants.Import);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        entity.Property(x => x.ImportBatchId)
            .IsRequired();

        entity.Property(x => x.RawDate)
            .IsRequired();

        entity.Property(x => x.RawAmount)
            .HasMaxLength(50)
            .IsRequired(false);

        entity.Property(x => x.RawDescription)
            .HasMaxLength(1000)
            .IsRequired(false);

        entity.Property(x => x.RawFitId)
            .HasMaxLength(256)
            .IsRequired(false);

        entity.Property(x => x.RawType)
            .HasMaxLength(50)
            .IsRequired(false);

        entity.Property(x => x.NormalizedDate)
            .IsRequired(false);

        entity.Property(x => x.AmountMinor)
            .IsRequired(false);

        entity.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired(false);

        entity.Property(x => x.Memo)
            .HasMaxLength(1000)
            .IsRequired(false);

        entity.Property(x => x.Status)
            .HasDefaultValue(TransactionImportRowStatusEnum.Unprocessed)
            .IsRequired();

        entity.Property(x => x.StatusReason)
            .HasMaxLength(500)
            .IsRequired(false);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<TransactionImportRow> entity);
}
