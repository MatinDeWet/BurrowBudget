using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Enums;
using Pagination.Models.Responses;
using Searchable.Core;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryGroupFeatures.GetCategoryGroupCategories;
internal sealed class GetCategoryGroupCategoriesHandler(ICategoryGroupQueryRepo categoryGroupQueryRepo, ICategoryQueryRepo categoryQueryRepo)
    : IQueryManager<GetCategoryGroupCategoriesRequest, PageableResponse<GetCategoryGroupCategoriesResponse>>
{
    public async Task<Result<PageableResponse<GetCategoryGroupCategoriesResponse>>> Handle(GetCategoryGroupCategoriesRequest request, CancellationToken cancellationToken)
    {
        bool groupExists = await categoryGroupQueryRepo.CategoryGroups
            .AnyAsync(x => x.Id == request.CategoryGroupId, cancellationToken);

        if (!groupExists)
        {
            return Result.NotFound($"Category Group with Id {request.CategoryGroupId} was not found.");
        }

        IQueryable<Category> query = categoryQueryRepo.Categories
            .Where(c => c.CategoryGroupId == request.CategoryGroupId);

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        query = query.FullTextSearch(request.SearchTerms);

        PageableResponse<GetCategoryGroupCategoriesResponse> response = await query
            .Select(c => new GetCategoryGroupCategoriesResponse
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
