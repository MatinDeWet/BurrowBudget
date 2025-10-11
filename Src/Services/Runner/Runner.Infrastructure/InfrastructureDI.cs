using System.Reflection;
using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Runner.Infrastructure.Data.Contexts;

namespace Runner.Infrastructure;
public static class InfrastructureDI
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
    {
        services.AddDbContext<BudgetContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseNpgsql(
                configuration.GetConnectionString("BudgetDb"),
                opt =>
                {
                    opt.MigrationsAssembly(typeof(BudgetContext).GetTypeInfo().Assembly.GetName().Name);
                    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemaConstants.Migrations);
                });

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            if (IsDevelopment)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}
