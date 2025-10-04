using Domain.Base.Implementation;

namespace Domain.Core.Entities;

public class User : Entity<Guid>
{
    public static User Create(Guid id)
    {
        return new User
        {
            Id = id
        };
    }
}
