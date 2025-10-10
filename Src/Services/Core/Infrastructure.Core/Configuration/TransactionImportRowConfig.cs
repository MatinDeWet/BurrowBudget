using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class TransactionImportRowConfig : IEntityTypeConfiguration<TransactionImportRow>
{
    public void Configure(EntityTypeBuilder<TransactionImportRow> entity)
    {
        entity.ToTable(nameof(TransactionImportRow), SchemaConstants.Import, t =>
        {
            t.HasCheckConstraint("ck_import_row_currency_len", "\"Currency\" IS NULL OR char_length(\"Currency\") = 3");
            t.HasCheckConstraint("ck_import_row_raw_currency_len", "\"RawCurrency\" IS NULL OR char_length(\"RawCurrency\") = 3");
        });

        entity.HasKey(x => x.Id);
        entity.HasIndex(x => new { x.ImportBatchId, x.RawLineNumber }).IsUnique();

        // Fast queue scans
        entity.HasIndex(x => new { x.ImportBatchId, x.Status });
        entity.HasIndex(x => new { x.Status, x.StatusAtUtc });

        // De-dup helpers
        entity.HasIndex(x => x.RawHash);
        entity.HasIndex(x => x.RawFitId);
        entity.HasIndex(x => x.ExternalId);

        // Optional partial unique: within a batch, don't allow same RawFitId twice (when present)
        entity.HasIndex(x => new { x.ImportBatchId, x.RawFitId })
         .IsUnique()
         .HasFilter("\"RawFitId\" IS NOT NULL");

        // Optional partial unique: within a batch, don't allow same RawHash twice (when present)
        entity.HasIndex(x => new { x.ImportBatchId, x.RawHash })
         .IsUnique()
         .HasFilter("\"RawHash\" IS NOT NULL");

        // Column types / lengths
        entity.Property(x => x.RawRecordJson).HasColumnType("jsonb").IsRequired();
        entity.Property(x => x.ErrorLogJson).HasColumnType("jsonb");

        entity.Property(x => x.RawDescription).HasMaxLength(2048);
        entity.Property(x => x.RawType).HasMaxLength(64);
        entity.Property(x => x.RawCurrency).HasMaxLength(3);
        entity.Property(x => x.RawCounterparty).HasMaxLength(512);
        entity.Property(x => x.RawReference).HasMaxLength(256);
        entity.Property(x => x.RawFitId).HasMaxLength(128);
        entity.Property(x => x.RawHash).HasMaxLength(64).IsRequired();

        entity.Property(x => x.Currency).HasMaxLength(3);
        entity.Property(x => x.Payee).HasMaxLength(512);
        entity.Property(x => x.Counterparty).HasMaxLength(512);
        entity.Property(x => x.Memo).HasMaxLength(1024);
        entity.Property(x => x.Reference).HasMaxLength(256);
        entity.Property(x => x.ExternalId).HasMaxLength(128);

        entity.Property(x => x.ErrorCode).HasMaxLength(64);
        entity.Property(x => x.ErrorMessage).HasMaxLength(2048);

        // Relationships
        entity.HasOne(x => x.ImportBatch)
         .WithMany(bh => bh.Rows)
         .HasForeignKey(x => x.ImportBatchId)
         .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<TransactionImportRow> entity);
}
