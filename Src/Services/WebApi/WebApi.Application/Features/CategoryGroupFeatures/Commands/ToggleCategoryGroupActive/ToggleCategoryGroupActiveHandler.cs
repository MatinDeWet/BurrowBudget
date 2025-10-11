using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryGroupFeatures.Commands.ToggleCategoryGroupActive;
internal sealed class ToggleCategoryGroupActiveHandler(ICategoryGroupQueryRepo queryRepo, ICategoryGroupCommandRepo commandRepo)
    : ICommandManager<ToggleCategoryGroupActiveRequest>
{
    public async Task<Result> Handle(ToggleCategoryGroupActiveRequest command, CancellationToken cancellationToken)
    {
        CategoryGroup? categoryGroup = await queryRepo.CategoryGroups.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (categoryGroup is null)
        {
            return Result.NotFound($"Category Group with Id {command.Id} was not found.");
        }

        categoryGroup.ToggleActive(command.State);

        await commandRepo.UpdateAsync(categoryGroup, true, cancellationToken);

        return Result.Success();
    }
}
