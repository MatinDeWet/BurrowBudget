using Infrastructure.Core.Constants;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.Data.Contexts;
public class BudgetContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public BudgetContext() { }

    public BudgetContext(DbContextOptions<BudgetContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(BudgetContext).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(SchemaConstants).Assembly);
    }
}
