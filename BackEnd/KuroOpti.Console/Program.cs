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
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appsettings.json", optional: true);
                        config.AddUserSecrets<Program>();
                    }
                )
                .ConfigureServices(
                    (context, services) =>
                    {
                        var connectionString = context.Configuration.GetConnectionString(
                            "DefaultConnection"
                        );

                        services.AddDbContext<KuroOptiDbContext>(options =>
                        {
                            options.UseMySql(
                                connectionString,
                                ServerVersion.AutoDetect(connectionString)
                            );
                        });
                        services.AddScoped<IFuelStationRepository, FuelStationRepository>();
                        services.AddHttpClient<EnaFuelPriceImporter>();
                        services.AddHttpClient<NominatimGeocodingService>();
                        services.AddScoped<IGeocodingService, NominatimGeocodingService>();
                        services.AddScoped<IFuelPriceImporter, EnaFuelPriceImporter>();
                    }
                );

            return host.Build();
        }
    }
}
