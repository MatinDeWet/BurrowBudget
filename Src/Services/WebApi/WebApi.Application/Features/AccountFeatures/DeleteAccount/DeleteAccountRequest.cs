namespace WebApi.Application.Features.AccountFeatures.DeleteAccount;
public sealed record DeleteAccountRequest(Guid Id) : ICommand;
