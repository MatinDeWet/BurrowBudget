namespace WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupCategoryCount;
public sealed record GetCategoryGroupCategoryCountRequest(Guid GroupId) : IQuery<int>;
