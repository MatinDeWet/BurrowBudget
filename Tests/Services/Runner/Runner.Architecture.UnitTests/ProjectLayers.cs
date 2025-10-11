using System.Reflection;
using Runner.Application;
using Runner.Domain;
using Runner.Infrastructure;
using Runner.Presentation;

namespace Runner.Architecture.UnitTests;

public abstract class ProjectLayers
{
    protected static readonly Assembly DomainAssembly = typeof(IDomainPointer).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IApplicationPointer).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(IInfrastructurePointer).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(IPresentationPointer).Assembly;
}
