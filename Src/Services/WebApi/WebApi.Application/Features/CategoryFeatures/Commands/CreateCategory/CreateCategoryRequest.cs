namespace WebApi.Application.Features.CategoryFeatures.Commands.CreateCategory;
public sealed record CreateCategoryRequest : ICommand<Guid>
{
    public Guid CategoryGroupId { get; init; }

    public string Name { get; init; }

    public string? Description { get; init; }

    public short SortOrder { get; init; } = 999;

    public bool IsActive { get; init; } = true;
}
