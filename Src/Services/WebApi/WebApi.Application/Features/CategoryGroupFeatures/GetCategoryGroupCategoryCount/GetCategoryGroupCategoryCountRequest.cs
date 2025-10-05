namespace WebApi.Application.Features.CategoryGroupFeatures.GetCategoryGroupCategoryCount;
public sealed record GetCategoryGroupCategoryCountRequest(Guid GroupId) : IQuery<int>;
