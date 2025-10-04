using System.Security.Claims;
using CQRS;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Identification.Core;
using Microsoft.EntityFrameworkCore;
using Repository.Core;
using WebApi.Application;
using WebApi.Infrastructure;
using WebApi.Infrastructure.Data.Contexts;
using WebApi.Presentation;
using WebApi.Presentation.Common.Filters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentitySupport();
builder.Services.AddIdentityPrepration();

builder.Services.AddCQRSSupport(typeof(IApplicationPointer));

builder.Services.AddDatabase(builder.Configuration, builder.Environment.IsDevelopment() || builder.Environment.IsStaging());
builder.Services.AddSecuredRepositories(typeof(IInfrastructurePointer));
builder.Services.AddRepositories(typeof(IInfrastructurePointer));

builder.Services
    .AddAuthenticationJwtBearer(o => o.SigningKey = builder.Configuration["Auth:JWTSigningKey"])
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.AutoTagPathSegmentIndex = 1;
        o.ShortSchemaNames = true;
    });

WebApplication app = builder.Build();

app.UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints(c =>
   {
       c.Endpoints.Configurator = ep => ep.Options(b => b.AddEndpointFilter<IdentityInfoFilter>());
       c.Endpoints.ShortNames = true;
       c.Endpoints.NameGenerator = ctx =>
       {
           if (ctx.EndpointType.Name.EndsWith("Endpoint", StringComparison.InvariantCultureIgnoreCase))
           {
               return ctx.EndpointType.Name[..^"Endpoint".Length];
           }
           else
           {
               return ctx.EndpointType.Name;
           }
       };
       c.Errors.UseProblemDetails();
       c.Security.RoleClaimType = ClaimTypes.Role;
   })
   .UseSwaggerGen(uiConfig: u => u.ShowOperationIDs());

ApplyDbMigrations(app);

app.Run();

static void ApplyDbMigrations(IApplicationBuilder app)
{
    using IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();

    if (serviceScope.ServiceProvider.GetRequiredService<BudgetContext>().Database.GetPendingMigrations().Any())
    {
        serviceScope.ServiceProvider.GetRequiredService<BudgetContext>().Database.Migrate();
    }
}
