namespace WebApi.Application.Features.CategoryFeatures.ChangeCategoryGroup;
public sealed record ChangeCategoryGroupRequest : ICommand
{
    public Guid CategoryId { get; init; }

    public Guid CategoryGroupId { get; init; }
}
