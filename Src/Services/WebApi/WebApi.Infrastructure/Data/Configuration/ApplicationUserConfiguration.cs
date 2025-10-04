using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.Data.Configuration;
internal partial class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.ToTable("AspNetUsers", SchemaConstants.Identity);

        entity
            .HasGeneratedTsVectorColumn(
            p => p.SearchVector,
            "english",
            p => new { p.Email, p.PhoneNumber })
            .HasIndex(e => e.SearchVector)
            .HasMethod("GIN");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ApplicationUser> entity);
}
