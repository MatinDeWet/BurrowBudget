using Ardalis.Result;

namespace CQRS.Contracts;
public interface IQueryManager<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery request, CancellationToken cancellationToken);
}
