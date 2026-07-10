// Add immediately after:
// var app = builder.Build();

const int migrationAttempts = 10;

for (int attempt = 1; attempt <= migrationAttempts; attempt++)
{
    try
    {
        using IServiceScope scope = app.Services.CreateScope();

        KuroOptiDbContext dbContext = scope.ServiceProvider.GetRequiredService<KuroOptiDbContext>();

        await dbContext.Database.MigrateAsync();

        app.Logger.LogInformation("Database migrations applied successfully");

        break;
    }
    catch (Exception exception) when (attempt < migrationAttempts)
    {
        app.Logger.LogWarning(
            exception,
            "Database migration attempt {Attempt}/{MaximumAttempts} failed. Retrying...",
            attempt,
            migrationAttempts
        );

        await Task.Delay(TimeSpan.FromSeconds(3));
    }
}
