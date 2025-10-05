using Domain.Base.Implementation;

namespace Domain.Core.Entities;
public class Category : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public short SortOrder { get; private set; } = 999;

    public bool IsActive { get; private set; } = true;

    public Guid CategoryGroupId { get; private set; }
    public virtual CategoryGroup CategoryGroup { get; private set; }

    public static Category Create(
        Guid userId,
        string name,
        Guid categoryGroupId,
        string? description = null,
        short sortOrder = 999,
        bool isActive = true)
    {
        return new Category
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Name = name,
            Description = description,
            SortOrder = sortOrder,
            IsActive = isActive,
            CategoryGroupId = categoryGroupId
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

    public void ChangeCategoryGroup(Guid categoryGroupId)
    {
        CategoryGroupId = categoryGroupId;
    }
}
