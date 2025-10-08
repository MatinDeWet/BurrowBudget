using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class TransactionImportFileConfig : IEntityTypeConfiguration<TransactionImportFile>
{
    public void Configure(EntityTypeBuilder<TransactionImportFile> entity)
    {
        entity.ToTable(nameof(TransactionImportFile), SchemaConstants.Import);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        entity.Property(x => x.ImportBatchId)
            .IsRequired();

        entity.Property(x => x.FullFileName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.FileName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.FileExtension)
            .HasMaxLength(16)
            .IsRequired();

        entity.Property(x => x.MimeType)
            .HasMaxLength(32)
            .IsRequired();

        entity.Property(x => x.BlobContainer)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(x => x.BlobName)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(x => x.Sha256)
            .HasMaxLength(64)
            .IsRequired()
            .IsFixedLength();

        entity.Property(x => x.SizeInBytes)
            .IsRequired();

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<TransactionImportFile> entity);
}
