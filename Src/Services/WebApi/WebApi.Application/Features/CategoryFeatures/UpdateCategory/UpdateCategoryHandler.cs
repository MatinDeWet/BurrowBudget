using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryFeatures.UpdateCategory;
internal sealed class UpdateCategoryHandler(ICategoryQueryRepo queryRepo, ICategoryCommandRepo commandRepo)
    : ICommandManager<UpdateCategoryRequest>
{
    public async Task<Result> Handle(UpdateCategoryRequest command, CancellationToken cancellationToken)
    {
        Category? category = await queryRepo.Categories.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (category is null)
        {
            return Result.NotFound($"Category with Id {command.Id} was not found.");
        }

        category.Update(
            command.Name,
            command.Description,
            command.SortOrder);

        await commandRepo.UpdateAsync(category, true, cancellationToken);

        return Result.Success();
    }
}
