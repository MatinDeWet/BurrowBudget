namespace WebApi.Application.Features.CategoryFeatures.GetCategoryById;
public sealed record GetCategoryByIdRequest(Guid Id) : IQuery<GetCategoryByIdResponse>;
