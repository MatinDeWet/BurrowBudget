using Blob.Integration;
using CQRS;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Repository.Core;
using Runner.Application;
using Runner.Infrastructure;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddCQRSSupport(typeof(IApplicationPointer));

builder.Services.AddDatabase(builder.Configuration, builder.Environment.IsDevelopment() || builder.Environment.IsStaging());
builder.Services.AddRepositories(typeof(IInfrastructurePointer));

builder.Services.AddBlobSupport(builder.Configuration);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
