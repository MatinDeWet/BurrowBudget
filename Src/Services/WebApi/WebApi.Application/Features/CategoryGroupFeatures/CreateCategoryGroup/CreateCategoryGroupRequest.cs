namespace WebApi.Application.Features.CategoryGroupFeatures.CreateCategoryGroup;
public sealed record CreateCategoryGroupRequest : ICommand<Guid>
{
    public string Name { get; init; }

    public string? Description { get; init; }

    public short SortOrder { get; init; } = 999;

    public bool IsActive { get; init; } = true;
}
