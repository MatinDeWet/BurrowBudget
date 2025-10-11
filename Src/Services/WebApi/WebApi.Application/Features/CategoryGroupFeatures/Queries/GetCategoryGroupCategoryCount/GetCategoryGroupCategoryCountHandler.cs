using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupCategoryCount;
internal sealed class GetCategoryGroupCategoryCountHandler(ICategoryGroupQueryRepo categoryGroupQueryRepo, ICategoryQueryRepo categoryQueryRepo)
    : IQueryManager<GetCategoryGroupCategoryCountRequest, int>
{
    public async Task<Result<int>> Handle(GetCategoryGroupCategoryCountRequest query, CancellationToken cancellationToken)
    {
        bool groupExists = await categoryGroupQueryRepo.CategoryGroups
            .AnyAsync(x => x.Id == query.GroupId, cancellationToken);

        if (!groupExists)
        {
            return Result.NotFound($"Category Group with Id {query.GroupId} was not found.");
        }

        int response = await categoryQueryRepo.Categories
            .Where(c => c.CategoryGroupId == query.GroupId)
            .CountAsync(cancellationToken);

        return response;
    }
}
