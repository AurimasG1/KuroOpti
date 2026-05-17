using KuroOpti.Data;
using KuroOpti.Repositories;
using KuroOpti.Repositories.Implementations;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KuroOpti.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = BuildHost();

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var dbContext = serviceProvider.GetRequiredService<KuroOptiDbContext>();
                dbContext.Database.Migrate();
                var userService = serviceProvider.GetRequiredService<IUserService>();
            }
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
                        services.AddScoped<IUserRepository, UserRepository>();
                        services.AddScoped<IFuelStationRepository, FuelStationRepository>();
                    }
                );

            return host.Build();
        }
    }
}
