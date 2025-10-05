using Domain.Base.Implementation;
using Domain.Core.Enums;
using NpgsqlTypes;
using Searchable.Domain;

namespace Domain.Core.Entities;
public class Account : Entity<Guid>, ISearchableEntity
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public AccountTypeEnum AccountType { get; private set; }

    public NpgsqlTsVector SearchVector { get; }
}
