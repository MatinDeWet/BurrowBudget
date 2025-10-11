using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryGroupFeatures.Queries.GetCategoryGroupById;
internal sealed class GetCategoryGroupByIdHandler(ICategoryGroupQueryRepo queryRepo)
    : IQueryManager<GetCategoryGroupByIdRequest, GetCategoryGroupByIdResponse>
{
    public async Task<Result<GetCategoryGroupByIdResponse>> Handle(GetCategoryGroupByIdRequest query, CancellationToken cancellationToken)
    {
        GetCategoryGroupByIdResponse? categoryGroup = await queryRepo.CategoryGroups
            .Where(x => x.Id == query.Id)
            .Select(x => new GetCategoryGroupByIdResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (categoryGroup is null)
        {
            return Result.NotFound($"Category Group with Id {query.Id} was not found.");
        }

        return categoryGroup;
    }
}
