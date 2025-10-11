namespace WebApi.Application.Features.CategoryFeatures.Commands.UpdateCategory;
public sealed record UpdateCategoryRequest : ICommand
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string? Description { get; init; }

    public short SortOrder { get; init; } = 999;
}
