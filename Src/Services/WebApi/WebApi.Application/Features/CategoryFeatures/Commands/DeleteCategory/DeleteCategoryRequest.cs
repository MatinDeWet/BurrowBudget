namespace WebApi.Application.Features.CategoryFeatures.Commands.DeleteCategory;
public sealed record DeleteCategoryRequest(Guid Id) : ICommand;
