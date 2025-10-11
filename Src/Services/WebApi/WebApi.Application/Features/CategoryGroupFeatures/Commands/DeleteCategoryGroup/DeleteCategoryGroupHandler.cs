using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryGroupFeatures.Commands.DeleteCategoryGroup;
internal sealed class DeleteCategoryGroupHandler(ICategoryGroupQueryRepo queryRepo, ICategoryGroupCommandRepo commandRepo)
    : ICommandManager<DeleteCategoryGroupRequest>
{
    public async Task<Result> Handle(DeleteCategoryGroupRequest command, CancellationToken cancellationToken)
    {
        CategoryGroup? categoryGroup = await queryRepo.CategoryGroups.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (categoryGroup is null)
        {
            return Result.NotFound($"Category Group with Id {command.Id} was not found.");
        }

        await commandRepo.DeleteAsync(categoryGroup, true, cancellationToken);

        return Result.Success();
    }
}
