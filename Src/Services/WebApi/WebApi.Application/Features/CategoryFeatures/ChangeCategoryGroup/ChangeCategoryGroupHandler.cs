using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryFeatures.ChangeCategoryGroup;
internal sealed class ChangeCategoryGroupHandler(ICategoryQueryRepo queryRepo, ICategoryCommandRepo commandRepo, ICategoryGroupQueryRepo groupQueryRepo)
    : ICommandManager<ChangeCategoryGroupRequest>
{
    public async Task<Result> Handle(ChangeCategoryGroupRequest command, CancellationToken cancellationToken)
    {
        bool groupExists = await groupQueryRepo.CategoryGroups.AnyAsync(x => x.Id == command.CategoryGroupId, cancellationToken);

        if (!groupExists)
        {
            return Result.NotFound($"Category Group with Id {command.CategoryGroupId} was not found.");
        }

        Category? category = await queryRepo.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.NotFound($"Category with Id {command.CategoryId} was not found.");
        }

        category.ChangeCategoryGroup(command.CategoryGroupId);

        await commandRepo.UpdateAsync(category, true, cancellationToken);

        return Result.Success();
    }
}
