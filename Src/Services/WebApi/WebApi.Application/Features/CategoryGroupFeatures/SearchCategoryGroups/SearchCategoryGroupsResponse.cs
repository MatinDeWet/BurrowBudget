namespace WebApi.Application.Features.CategoryGroupFeatures.SearchCategoryGroups;
public sealed record SearchCategoryGroupsResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public short SortOrder { get; init; } = 999;

    public bool IsActive { get; init; } = true;
}
