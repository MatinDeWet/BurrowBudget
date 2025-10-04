using System.Reflection;
using WebApi.Application;
using WebApi.Domain;
using WebApi.Infrastructure;
using WebApi.Presentation;

namespace WebApi.Architecture.UnitTests;

public abstract class ProjectLayers
{
    protected static readonly Assembly DomainAssembly = typeof(IDomainPointer).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IApplicationPointer).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(IInfrastructurePointer).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(IPresentationPointer).Assembly;
}
