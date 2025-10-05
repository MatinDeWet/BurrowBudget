namespace WebApi.Application.Features.CategoryGroupFeatures.GetCategoryGroupById;
public sealed record GetCategoryGroupByIdRequest(Guid Id) : IQuery<GetCategoryGroupByIdResponse>;
