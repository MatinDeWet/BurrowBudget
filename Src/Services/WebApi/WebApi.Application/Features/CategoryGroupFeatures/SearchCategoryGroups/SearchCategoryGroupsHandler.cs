using Ardalis.Result;
using Domain.Core.Entities;
using Pagination;
using Pagination.Enums;
using Pagination.Models.Responses;
using Searchable.Core;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryGroupFeatures.SearchCategoryGroups;
internal sealed class SearchCategoryGroupsHandler(ICategoryGroupQueryRepo queryRepo)
    : IQueryManager<SearchCategoryGroupsRequest, PageableResponse<SearchCategoryGroupsResponse>>
{
    public async Task<Result<PageableResponse<SearchCategoryGroupsResponse>>> Handle(SearchCategoryGroupsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<CategoryGroup> query = queryRepo.CategoryGroups;

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        query = query.FullTextSearch(request.SearchTerms);

        PageableResponse<SearchCategoryGroupsResponse> response = await query
            .Select(c => new SearchCategoryGroupsResponse
            {
                Id = c.Id,
                Name = c.Name,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive
            })
            .ToPageableListAsync(x => x.SortOrder, OrderDirectionEnum.Ascending, request, cancellationToken);

        return response;
    }
}
