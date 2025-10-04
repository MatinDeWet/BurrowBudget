using CQRS.Behaviors;
using CQRS.Contracts;
using CQRS.Implementation;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace CQRS;
public static class CQRSDI
{
    public static IServiceCollection AddCQRSSupport(this IServiceCollection services, Type assemplyPointer)
    {
        return services.AddCQRSSupport(assemplyPointer, null);
    }

    public static IServiceCollection AddCQRSSupport(this IServiceCollection services, Type assemplyPointer, Action<IServiceCollection>? configureDecorators)
    {
        services.Scan(scan => scan.FromAssembliesOf(assemplyPointer)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryManager<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandManager<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandManager<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        // Apply decorators only if there are implementations registered
        services.TryDecorateIfImplementationsExist(typeof(IQueryManager<,>), typeof(LoggingDecorator.QueryManager<,>));
        services.TryDecorateIfImplementationsExist(typeof(ICommandManager<,>), typeof(LoggingDecorator.CommandManager<,>));
        services.TryDecorateIfImplementationsExist(typeof(ICommandManager<>), typeof(LoggingDecorator.CommandBaseManager<>));

        // Apply additional decorators if provided
        configureDecorators?.Invoke(services);

        services.Scan(scan => scan.FromAssembliesOf(assemplyPointer)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventManager<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.AddScoped<ISaveChangesInterceptor, DomainEventsInterceptor>();

        return services;
    }

    /// <summary>
    /// Safely decorates a service type only if implementations are registered for that type.
    /// This prevents DI container errors when no implementations exist.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="serviceType">The service type to decorate (generic type definition)</param>
    /// <param name="decoratorType">The decorator type (generic type definition)</param>
    public static void TryDecorateIfImplementationsExist(this IServiceCollection services, Type serviceType, Type decoratorType)
    {
        // Check if there are any registered implementations for the service type
        bool hasImplementations = services.Any(descriptor =>
            descriptor.ServiceType.IsGenericType &&
            descriptor.ServiceType.GetGenericTypeDefinition() == serviceType);

        if (hasImplementations)
        {
            services.Decorate(serviceType, decoratorType);
        }
    }
}
