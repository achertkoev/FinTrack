using Microsoft.EntityFrameworkCore;
using MigrationService.Infrastructure.Persistence;

namespace MigrationService;

public static class Extensions
{
    public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.MigrateAsync().GetAwaiter().GetResult();

        return app;
    }
}