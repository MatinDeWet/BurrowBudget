namespace WebApi.Application.Features.CategoryFeatures.Commands.ToggleCategoryActive;
public sealed record ToggleCategoryActiveRequest : ICommand
{
    public Guid Id { get; init; }

    public bool? State { get; init; }
}
