using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;

public static class Extentions
{
    public static async Task<IApplicationBuilder> UseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();
        var dbContextsTypes = typeof(Program).Assembly.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(DbContext)) && type.IsClass);

        foreach (var dbContextType in dbContextsTypes)
        {
            await using var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;

            if (dbContext is null) continue;

            try
            {
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database.");
                throw;
            }
        }

        return app;
    }
}