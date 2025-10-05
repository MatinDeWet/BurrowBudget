namespace WebApi.Application.Features.AccountFeatures.GetAccountById;
public sealed record GetAccountByIdRequest(Guid Id) : IQuery<GetAccountByIdResponse>;
