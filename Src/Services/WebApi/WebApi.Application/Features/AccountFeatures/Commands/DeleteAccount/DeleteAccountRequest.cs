namespace WebApi.Application.Features.AccountFeatures.Commands.DeleteAccount;
public sealed record DeleteAccountRequest(Guid Id) : ICommand;
