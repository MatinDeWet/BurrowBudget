using Domain.Core.Entities;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable(nameof(User), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<User> entity);
}
