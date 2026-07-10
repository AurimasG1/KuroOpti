using KuroOpti.Data;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.API.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(
        this WebApplication app)
    {
        const int maximumAttempts = 10;

        for (int attempt = 1; attempt <= maximumAttempts; attempt++)
        {
            try
            {
                await using AsyncServiceScope scope =
                    app.Services.CreateAsyncScope();

                KuroOptiDbContext dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<KuroOptiDbContext>();

                await dbContext.Database.MigrateAsync();

                app.Logger.LogInformation(
                    "Database migrations applied successfully");

                return;
            }
            catch (Exception exception)
                when (attempt < maximumAttempts)
            {
                app.Logger.LogWarning(
                    exception,
                    "Database migration attempt {Attempt}/{MaximumAttempts} failed. Retrying...",
                    attempt,
                    maximumAttempts);

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }

        throw new InvalidOperationException(
            "Database migrations could not be applied");
    }
}
