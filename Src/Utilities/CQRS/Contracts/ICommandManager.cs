using Ardalis.Result;

namespace CQRS.Contracts;

public interface ICommandManager<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand request, CancellationToken cancellationToken);
}

public interface ICommandManager<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand request, CancellationToken cancellationToken);
}
