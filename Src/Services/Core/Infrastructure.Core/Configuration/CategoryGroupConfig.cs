using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class CategoryGroupConfig : IEntityTypeConfiguration<CategoryGroup>
{
    public void Configure(EntityTypeBuilder<CategoryGroup> entity)
    {
        entity.ToTable(nameof(CategoryGroup), SchemaConstants.Default);

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
            .WithMany(p => p.CategoryGroups)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<CategoryGroup> entity);
}
