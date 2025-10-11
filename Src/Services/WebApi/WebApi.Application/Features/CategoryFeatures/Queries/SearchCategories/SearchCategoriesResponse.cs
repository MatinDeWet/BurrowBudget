using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Core.Entities;

namespace WebApi.Application.Features.CategoryFeatures.Queries.SearchCategories;
public sealed record SearchCategoriesResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public short SortOrder { get; init; } = 999;

    public bool IsActive { get; init; } = true;

    public Guid CategoryGroupId { get; init; }
    public string CategoryGroupName { get; init; }
}
