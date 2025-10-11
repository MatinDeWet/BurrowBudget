using Ardalis.Result;
using Domain.Core.Entities;
using Pagination;
using Pagination.Enums;
using Pagination.Models.Responses;
using Searchable.Core;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryFeatures.Queries.SearchCategories;
internal sealed class SearchCategoriesHandler(ICategoryQueryRepo queryRepo)
    : IQueryManager<SearchCategoriesRequest, PageableResponse<SearchCategoriesResponse>>
{
    public async Task<Result<PageableResponse<SearchCategoriesResponse>>> Handle(SearchCategoriesRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Category> query = queryRepo.Categories;

        if (request.CategoryGroupId.HasValue)
        {
            query = query.Where(c => c.CategoryGroupId == request.CategoryGroupId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        query = query.FullTextSearch(request.SearchTerms);

        PageableResponse<SearchCategoriesResponse> response = await query
            .Select(c => new SearchCategoriesResponse
            {
                Id = c.Id,
                Name = c.Name,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                CategoryGroupId = c.CategoryGroupId,
                CategoryGroupName = c.CategoryGroup.Name,
            })
            .ToPageableListAsync(x => x.SortOrder, OrderDirectionEnum.Ascending, request, cancellationToken);

        return response;
    }
}
