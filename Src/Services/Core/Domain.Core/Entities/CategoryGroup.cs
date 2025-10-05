using Domain.Base.Implementation;

namespace Domain.Core.Entities;
public class CategoryGroup : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public short SortOrder { get; private set; } = 999;

    public bool IsActive { get; private set; } = true;

    public virtual ICollection<Category> Categories { get; private set; } = new List<Category>();

    public static CategoryGroup Create(
        Guid userId,
        string name,
        string? description = null,
        short sortOrder = 999,
        bool isActive = true)
    {
        return new CategoryGroup
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Name = name,
            Description = description,
            SortOrder = sortOrder,
            IsActive = isActive
        };
    }

    public void Update(
        string name,
        string? description = null,
        short sortOrder = 999)
    {
        Name = name;
        Description = description;
        SortOrder = sortOrder;
    }

    public void ToggleActive(bool? state)
    {
        if (state.HasValue)
        {
            IsActive = state.Value;
        }

        IsActive = !IsActive;
    }
}
