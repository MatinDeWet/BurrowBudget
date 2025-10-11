namespace WebApi.Application.Features.CategoryFeatures.Queries.GetCategoryById;
public sealed record GetCategoryByIdRequest(Guid Id) : IQuery<GetCategoryByIdResponse>;
