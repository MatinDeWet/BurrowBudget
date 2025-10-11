using Ardalis.Result;
using Domain.Core.Entities;
using Identification.Base;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryFeatures.Commands.CreateCategory;
internal sealed class CreateCategoryHandler(ICategoryGroupQueryRepo queryRepo, ICategoryCommandRepo commandRepo, IIdentityInfo identityInfo)
    : ICommandManager<CreateCategoryRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryRequest command, CancellationToken cancellationToken)
    {
        bool groupExists = await queryRepo.CategoryGroups.AnyAsync(x => x.Id == command.CategoryGroupId, cancellationToken);

        if (!groupExists)
        {
            return Result.NotFound($"Category Group with Id {command.CategoryGroupId} was not found.");
        }

        var category = Category.Create(
            identityInfo.GetIdentityId(),
            command.Name,
            command.CategoryGroupId,
            command.Description,
            command.SortOrder,
            command.IsActive);

        await commandRepo.InsertAsync(category, true, cancellationToken);

        return category.Id;
    }
}
