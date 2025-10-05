namespace WebApi.Application.Features.CategoryFeatures.DeleteCategory;
public sealed record DeleteCategoryRequest(Guid Id) : ICommand;
