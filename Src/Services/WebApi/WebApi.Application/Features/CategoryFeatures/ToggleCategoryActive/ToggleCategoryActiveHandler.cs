using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryFeatures.ToggleCategoryActive;
internal sealed class ToggleCategoryActiveHandler(ICategoryQueryRepo queryRepo, ICategoryCommandRepo commandRepo)
    : ICommandManager<ToggleCategoryActiveRequest>
{
    public async Task<Result> Handle(ToggleCategoryActiveRequest command, CancellationToken cancellationToken)
    {
        Category? category = await queryRepo.Categories.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (category is null)
        {
            return Result.NotFound($"Category with Id {command.Id} was not found.");
        }

        category.ToggleActive(command.IsActive);

        await commandRepo.UpdateAsync(category, true, cancellationToken);

        return Result.Success();
    }
}
