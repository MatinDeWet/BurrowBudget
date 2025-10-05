using Ardalis.Result;
using CQRS.Contracts;
using WebApi.Presentation.Common.Extensions;

namespace WebApi.Presentation.Common.Helpers;

public abstract class CommandEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : CQRS.Contracts.ICommand<TResponse>
    where TResponse : notnull
{
    private readonly ICommandManager<TRequest, TResponse> _manager;

    protected CommandEndpoint(ICommandManager<TRequest, TResponse> manager)
    {
        _manager = manager;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Result<TResponse> result = await _manager.Handle(req, ct);
        await this.SendResponse(result);
    }
}

public abstract class CommandEndpoint<TRequest> : Endpoint<TRequest>
    where TRequest : CQRS.Contracts.ICommand
{
    private readonly ICommandManager<TRequest> _manager;

    protected CommandEndpoint(ICommandManager<TRequest> manager)
    {
        _manager = manager;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Result result = await _manager.Handle(req, ct);
        await this.SendResponse(result);
    }
}

public abstract class QueryEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : CQRS.Contracts.IQuery<TResponse>
    where TResponse : notnull
{
    private readonly IQueryManager<TRequest, TResponse> _manager;

    protected QueryEndpoint(IQueryManager<TRequest, TResponse> manager)
    {
        _manager = manager;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Result<TResponse> result = await _manager.Handle(req, ct);
        await this.SendResponse(result);
    }
}

public abstract class QueryWithoutRequestEndpoint<TRequest, TResponse> : EndpointWithoutRequest<TResponse>
    where TRequest : CQRS.Contracts.IQuery<TResponse>, new()
    where TResponse : notnull
{
    private readonly IQueryManager<TRequest, TResponse> _manager;

    protected QueryWithoutRequestEndpoint(IQueryManager<TRequest, TResponse> manager)
    {
        _manager = manager;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var request = new TRequest();
        Result<TResponse> result = await _manager.Handle(request, ct);
        await this.SendResponse(result);
    }
}
