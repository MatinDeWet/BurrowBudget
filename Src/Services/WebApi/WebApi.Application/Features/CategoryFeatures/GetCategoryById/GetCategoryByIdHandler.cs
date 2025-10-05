using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryFeatures.GetCategoryById;
internal sealed class GetCategoryByIdHandler(ICategoryQueryRepo queryRepo)
    : IQueryManager<GetCategoryByIdRequest, GetCategoryByIdResponse>
{
    public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdRequest query, CancellationToken cancellationToken)
    {
        GetCategoryByIdResponse? category = await queryRepo.Categories
            .Where(x => x.Id == query.Id)
            .Select(x => new GetCategoryByIdResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive,
                CategoryGroupId = x.CategoryGroupId,
                CategoryGroupName = x.CategoryGroup.Name,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return Result.NotFound($"Category with Id {query.Id} was not found.");
        }

        return category;
    }
}
