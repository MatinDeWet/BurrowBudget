using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryGroupFeatures.UpdateCategoryGroup;
internal sealed class UpdateCategoryGroupHandler(ICategoryGroupQueryRepo queryRepo, ICategoryGroupCommandRepo commandRepo)
    : ICommandManager<UpdateCategoryGroupRequest>
{
    public async Task<Result> Handle(UpdateCategoryGroupRequest command, CancellationToken cancellationToken)
    {
        CategoryGroup? categoryGroup = await queryRepo.CategoryGroups.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (categoryGroup is null)
        {
            return Result.NotFound($"Category Group with Id {command.Id} was not found.");
        }

        categoryGroup.Update(
            command.Name,
            command.Description,
            command.SortOrder);

        await commandRepo.UpdateAsync(categoryGroup, true, cancellationToken);

        return Result.Success();
    }
}
