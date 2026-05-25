using KuroOpti.Data;
using KuroOpti.Repositories;
using KuroOpti.Services.Implementations;
using KuroOpti.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KuroOpti.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = BuildHost();
            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            // Migracijos
            var dbContext = serviceProvider.GetRequiredService<KuroOptiDbContext>();
            await dbContext.Database.MigrateAsync();

            // Importeris
            IFuelPriceImporter importer = serviceProvider.GetRequiredService<IFuelPriceImporter>();
            await importer.ImportAsync();
        }

        public static IHost BuildHost()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(
                    (context, config) =>
                    {
                        var env =
                            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                            ?? "Development";

                        var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;

                        config.SetBasePath(basePath);
                        config.AddJsonFile("appsettings.json", optional: false);
                        config.AddJsonFile($"appsettings.{env}.json", optional: true);
                        config.AddEnvironmentVariables();
                    }
                )
                .ConfigureServices(
                    (context, services) =>
                    {
                        var connectionString = context.Configuration.GetConnectionString(
                            "DefaultConnection"
                        );

                        System.Console.WriteLine("DB CONNECTION: " + connectionString);

                        if (string.IsNullOrWhiteSpace(connectionString))
                            throw new Exception("Connection string is NULL. Config not loaded.");

                        services.AddDbContext<KuroOptiDbContext>(options =>
                        {
                            options.UseMySql(
                                connectionString,
                                ServerVersion.AutoDetect(connectionString)
                            );
                        });

                        services.AddScoped<IFuelStationRepository, FuelStationRepository>();
                        services.AddHttpClient<EnaFuelPriceImporter>();
                        services.AddHttpClient<GoogleGeocodingService>();
                        services.AddScoped<IGeocodingService, GoogleGeocodingService>();
                        services.AddScoped<IFuelPriceImporter, EnaFuelPriceImporter>();
                    }
                );

            return host.Build();
        }
    }
}
