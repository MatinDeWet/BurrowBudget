using Ardalis.Result;
using CQRS.Contracts;
using WebApi.Presentation.Common.Extensions;

namespace WebApi.Presentation.Common.Helpers;

/// <summary>
/// Base endpoint class that provides common functionality for CQRS operations
/// </summary>
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Executes a query and sends the response
    /// </summary>
    protected async Task ExecuteQueryAsync<TQuery, TResult>(
        IQueryManager<TQuery, TResult> manager,
        TQuery query,
        CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        Result<TResult> result = await manager.Handle(query, ct);
        await this.SendResponse(result);
    }

    /// <summary>
    /// Executes a command that returns a value and sends the response
    /// </summary>
    protected async Task ExecuteCommandAsync<TCommand, TResult>(
        ICommandManager<TCommand, TResult> manager,
        TCommand command,
        CancellationToken ct)
        where TCommand : CQRS.Contracts.ICommand<TResult>
    {
        Result<TResult> result = await manager.Handle(command, ct);
        await this.SendResponse(result);
    }
}

/// <summary>
/// Base endpoint class for endpoints without request body
/// </summary>
public abstract class BaseEndpointWithoutRequest<TResponse> : EndpointWithoutRequest<TResponse>
{
    /// <summary>
    /// Executes a query and sends the response
    /// </summary>
    protected async Task ExecuteQueryAsync<TQuery, TResult>(
        IQueryManager<TQuery, TResult> manager,
        TQuery query,
        CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        Result<TResult> result = await manager.Handle(query, ct);
        await this.SendResponse(result);
    }
}

/// <summary>
/// Base endpoint class for command operations (no response body)
/// </summary>
public abstract class BaseCommandEndpoint<TRequest> : Endpoint<TRequest>
    where TRequest : notnull
{
    /// <summary>
    /// Executes a command and sends the response
    /// </summary>
    protected async Task ExecuteCommandAsync<TCommand>(
        ICommandManager<TCommand> manager,
        TCommand command,
        CancellationToken ct)
        where TCommand : CQRS.Contracts.ICommand
    {
        Result result = await manager.Handle(command, ct);
        await this.SendResponse(result);
    }

    /// <summary>
    /// Executes a command that returns a value and sends the response
    /// </summary>
    protected async Task ExecuteCommandAsync<TCommand, TResult>(
        ICommandManager<TCommand, TResult> manager,
        TCommand command,
        CancellationToken ct)
        where TCommand : CQRS.Contracts.ICommand<TResult>
    {
        Result<TResult> result = await manager.Handle(command, ct);
        await this.SendResponse(result);
    }
}
