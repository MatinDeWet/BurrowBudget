using Domain.Core.Constants;
using Microsoft.AspNetCore.Identity;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data.Contexts;

namespace WebApi.Presentation;

public static class PresentationDI
{
    public static IServiceCollection AddIdentityPrepration(this IServiceCollection services)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(AuthConstants.LoginProvider)
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<BudgetContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
