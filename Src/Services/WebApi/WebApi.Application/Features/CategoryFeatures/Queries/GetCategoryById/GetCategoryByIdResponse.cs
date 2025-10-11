namespace WebApi.Application.Features.CategoryFeatures.Queries.GetCategoryById;
public sealed record GetCategoryByIdResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string? Description { get; init; }

    public short SortOrder { get; init; } = 999;

    public bool IsActive { get; init; } = true;

    public Guid CategoryGroupId { get; init; }

    public string CategoryGroupName { get; init; }
}
