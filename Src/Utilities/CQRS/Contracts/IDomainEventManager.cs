using Domain.Base.Contracts;

namespace CQRS.Contracts;
public interface IDomainEventManager<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}
