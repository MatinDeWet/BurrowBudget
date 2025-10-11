namespace WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupById;
public sealed record GetCategoryGroupByIdRequest(Guid Id) : IQuery<GetCategoryGroupByIdResponse>;
