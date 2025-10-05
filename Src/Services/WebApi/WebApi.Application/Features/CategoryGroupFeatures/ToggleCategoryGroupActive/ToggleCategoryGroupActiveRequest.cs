namespace WebApi.Application.Features.CategoryGroupFeatures.ToggleCategoryGroupActive;
public sealed record ToggleCategoryGroupActiveRequest : ICommand
{
    public Guid Id { get; init; }

    public bool? State { get; init; }
}
