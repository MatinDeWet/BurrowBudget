using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class TransactionImportBatchConfig : IEntityTypeConfiguration<TransactionImportBatch>
{
    public void Configure(EntityTypeBuilder<TransactionImportBatch> entity)
    {
        entity.ToTable(nameof(TransactionImportBatch), SchemaConstants.Import);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        entity.Property(x => x.AccountId)
            .IsRequired();

        entity.Property(x => x.ImportedAt)
            .IsRequired();

        entity.Property(x => x.Status)
            .IsRequired();

        entity.Property(x => x.RetryCount)
            .HasDefaultValue(0)
            .IsRequired();

        entity.Property(x => x.Error)
            .HasMaxLength(2000)
            .IsRequired(false);

        entity.Property(x => x.UploadedAt)
            .IsRequired(false);

        entity.Property(x => x.QueuedAt)
            .IsRequired(false);

        entity.Property(x => x.StartedAt)
            .IsRequired(false);

        entity.Property(x => x.CompletedAt)
            .IsRequired(false);

        entity.Property(x => x.FailedAt)
            .IsRequired(false);

        entity.Property(x => x.CanceledAt)
            .IsRequired(false);

        entity.Property(x => x.SupersededAt)
            .IsRequired(false);

        entity.Property(x => x.Version)
            .IsRowVersion();

        entity.HasOne(d => d.Account)
            .WithMany(d => d.TransactionImportBatches)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(d => d.Rows)
            .WithOne(p => p.ImportBatch)
            .HasForeignKey(p => p.ImportBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.File)
            .WithOne(p => p.ImportBatch)
            .HasForeignKey<TransactionImportFile>(p => p.ImportBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<TransactionImportBatch> entity);
}
