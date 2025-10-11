using Infrastructure.Core.Constants;
using Microsoft.EntityFrameworkCore;

namespace Runner.Infrastructure.Data.Contexts;
public class BudgetContext : DbContext
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
