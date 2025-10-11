using Blob.Integration;
using CQRS;
using Repository.Core;
using Runner.Application;
using Runner.Infrastructure;
using Runner.Presentation;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCQRSSupport(typeof(IApplicationPointer));

builder.Services.AddDatabase(builder.Configuration, builder.Environment.IsDevelopment() || builder.Environment.IsStaging());
builder.Services.AddRepositories(typeof(IInfrastructurePointer));

builder.Services.AddBlobSupport(builder.Configuration);

builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();
