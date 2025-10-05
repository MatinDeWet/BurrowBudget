using Ardalis.Result;
using Domain.Core.Entities;
using Identification.Base;
using WebApi.Application.Repositories.Command;

namespace WebApi.Application.Features.CategoryGroupFeatures.CreateCategoryGroup;
internal sealed class CreateCategoryGroupHandler(ICategoryGroupCommandRepo commandRepo, IIdentityInfo identityInfo)
    : ICommandManager<CreateCategoryGroupRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryGroupRequest command, CancellationToken cancellationToken)
    {
        var categoryGroup = CategoryGroup.Create(
            identityInfo.GetIdentityId(),
            command.Name,
            command.Description,
            command.SortOrder,
            command.IsActive);

        await commandRepo.InsertAsync(categoryGroup, true, cancellationToken);

        return categoryGroup.Id;
    }
}
