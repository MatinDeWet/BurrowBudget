using Microsoft.AspNetCore.Identity;
using NpgsqlTypes;
using Searchable.Domain;

namespace WebApi.Domain.Entities;
public class ApplicationUser : IdentityUser<Guid>, ISearchableEntity
{
    public NpgsqlTsVector SearchVector { get; } = default!;

    public virtual ICollection<UserRefreshToken> RefreshTokens { get; private set; } = [];
}
