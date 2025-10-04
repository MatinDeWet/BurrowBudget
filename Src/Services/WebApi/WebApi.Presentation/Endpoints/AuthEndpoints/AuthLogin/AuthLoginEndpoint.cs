using System.Security.Claims;
using Ardalis.Result;
using CQRS.Contracts;
using Microsoft.AspNetCore.Identity;
using WebApi.Application.Features.AuthFeatures.Queries.GetUserClaims;
using WebApi.Domain.Entities;
using WebApi.Presentation.Common.Extensions;
using WebApi.Presentation.Endpoints.AuthEndpoints.Common;

namespace WebApi.Presentation.Endpoints.AuthEndpoints.AuthLogin;

public class AuthLoginEndpoint(UserManager<ApplicationUser> userManager, IQueryManager<GetUserClaimsRequest, List<Claim>> userClaimHandler) : Endpoint<AuthLoginRequest, ApplicationTokenResponse>
{
    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
        Summary(x =>
        {
            x.Summary = "Login to the application";
            x.Description = "This endpoint allows users to log in using their email and password.";
            x.Response<ApplicationTokenResponse>(StatusCodes.Status200OK, "Login successful");
            x.Response(StatusCodes.Status400BadRequest, "Invalid request");
            x.Response(StatusCodes.Status401Unauthorized, "Unauthorized - Invalid Email / Password");
        });
    }

    public override async Task HandleAsync(AuthLoginRequest req, CancellationToken ct)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(req.Email);

        if (user is null)
        {
            ThrowError("Invalid Email / Password", StatusCodes.Status401Unauthorized);
        }

        bool isValidPassword = await userManager.CheckPasswordAsync(user, req.Password);

        if (!isValidPassword)
        {
            ThrowError("Invalid Email / Password", StatusCodes.Status401Unauthorized);
        }

        Result<List<Claim>> result = await userClaimHandler.Handle(new GetUserClaimsRequest(user.Id), ct);

        await this.SendResponseAsync(result, async claims => await CreateTokenWith<UserTokenService>(user.Id.ToString(), options => options.Claims.AddRange(claims.Value)));
    }
}
