using Domain.Base.Implementation;

namespace Domain.Core.Entities;

public class User : Entity<Guid>
{
    public virtual ICollection<CategoryGroup> CategoryGroups { get; private set; } = [];

    public virtual ICollection<Category> Categories { get; private set; } = [];

    public virtual ICollection<Account> Accounts { get; private set; } = [];

    public static User Create(Guid id)
    {
        return new User
        {
            Id = id
        };
    }
}
