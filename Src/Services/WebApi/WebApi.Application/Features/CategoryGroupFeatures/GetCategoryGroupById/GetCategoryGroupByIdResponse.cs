namespace WebApi.Application.Features.CategoryGroupFeatures.GetCategoryGroupById;
public sealed record GetCategoryGroupByIdResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string? Description { get; init; }

    public short SortOrder { get; init; } = 999;

    public bool IsActive { get; init; } = true;
}
