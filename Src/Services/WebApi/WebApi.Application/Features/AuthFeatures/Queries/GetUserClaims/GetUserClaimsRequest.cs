using System.Security.Claims;

namespace WebApi.Application.Features.AuthFeatures.Queries.GetUserClaims;
public sealed record GetUserClaimsRequest(Guid UserId) : IQuery<List<Claim>>;
