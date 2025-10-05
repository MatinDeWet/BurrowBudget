using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        entity.ToTable(nameof(Category), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        entity.Property(x => x.Name)
            .HasMaxLength(32)
            .IsRequired();

        entity.Property(x => x.Description)
            .HasMaxLength(256)
            .IsRequired(false);

        entity.Property(x => x.SortOrder)
            .HasDefaultValue(999)
            .IsRequired();

        entity.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        entity.HasGeneratedTsVectorColumn(
            p => p.SearchVector,
            "english",
            p => new { p.Name, p.Description })
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");

        entity.HasOne(d => d.User)
            .WithMany(p => p.Categories)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.CategoryGroup)
            .WithMany(p => p.Categories)
            .HasForeignKey(d => d.CategoryGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<Category> entity);
}
