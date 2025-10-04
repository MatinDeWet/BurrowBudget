using System.Security.Claims;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using WebApi.Domain.Entities;

namespace WebApi.Application.Features.AuthFeatures.Queries.GetUserClaims;
internal sealed class GetUserClaimsHandler(UserManager<ApplicationUser> userManager) : IQueryManager<GetUserClaimsRequest, List<Claim>>
{
    public async Task<Result<List<Claim>>> Handle(GetUserClaimsRequest query, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(query.UserId.ToString());

        if (user is null)
        {
            return Result.NotFound();
        }

        IList<Claim> userClaims = await userManager.GetClaimsAsync(user);

        IList<string> roles = await userManager.GetRolesAsync(user);
        IList<Claim> roleClaims = [.. roles.Select(role => new Claim(ClaimTypes.Role, role))];


        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Email, user.Email ?? string.Empty)
        }
        .Union(roleClaims)
        .Union(userClaims)
        .ToList();

        return claims;
    }
}
