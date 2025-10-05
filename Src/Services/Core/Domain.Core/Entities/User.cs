using Domain.Base.Implementation;

namespace Domain.Core.Entities;

public class User : Entity<Guid>
{
    public virtual ICollection<CategoryGroup> CategoryGroups { get; private set; } = new List<CategoryGroup>();

    public virtual ICollection<Category> Categories { get; private set; } = new List<Category>();

    public static User Create(Guid id)
    {
        return new User
        {
            Id = id
        };
    }
}
