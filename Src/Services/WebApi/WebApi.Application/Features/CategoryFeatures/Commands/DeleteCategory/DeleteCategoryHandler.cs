using Ardalis.Result;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.CategoryFeatures.Commands.DeleteCategory;
internal sealed class DeleteCategoryHandler(ICategoryQueryRepo queryRepo, ICategoryCommandRepo commandRepo)
    : ICommandManager<DeleteCategoryRequest>
{
    public async Task<Result> Handle(DeleteCategoryRequest command, CancellationToken cancellationToken)
    {
        Category? category = await queryRepo.Categories.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (category is null)
        {
            return Result.NotFound($"Category with Id {command.Id} was not found.");
        }

        await commandRepo.DeleteAsync(category, true, cancellationToken);

        return Result.Success();
    }
}
