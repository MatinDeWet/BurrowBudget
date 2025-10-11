namespace WebApi.Application.Features.AccountFeatures.Queries.GetAccountById;
public sealed record GetAccountByIdRequest(Guid Id) : IQuery<GetAccountByIdResponse>;
