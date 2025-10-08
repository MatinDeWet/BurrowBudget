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

    public List<TransactionImportBatch> TransactionImportBatches { get; private set; } = [];

    public static Account Create(
        Guid userId,
        string name,
        AccountTypeEnum accountType,
        string? description = null)
    {
        return new Account
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Name = name,
            Description = description,
            AccountType = accountType
        };
    }

    public void Update(
        string name,
        AccountTypeEnum accountType,
        string? description = null)
    {
        Name = name;
        AccountType = accountType;
        Description = description;
    }
}
