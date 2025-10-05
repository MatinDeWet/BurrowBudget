namespace WebApi.Application.Features.CategoryFeatures.ToggleCategoryActive;
public sealed record ToggleCategoryActiveRequest : ICommand
{
    public Guid Id { get; init; }

    public bool? IsActive { get; init; }
}
