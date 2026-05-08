using KuroOpti.Data;
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
                // dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
                // var productService = serviceProvider.GetRequiredService<IProductService>();
                // var basketService = serviceProvider.GetRequiredService<IBasketService>();
                // var userService = serviceProvider.GetRequiredService<IVartotojasService>();
                // var orderService = serviceProvider.GetRequiredService<IOrderService>();
                // var orderRepo = serviceProvider.GetRequiredService<IOrderRepository>();
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
                        // services.AddScoped<IProductRepository, ProductRepository>();
                        // services.AddScoped<IBasketRepository, BasketRepository>();
                        // services.AddScoped<IProductService, ProductService>();
                        // services.AddScoped<IBasketService, BasketService>();
                        // services.AddScoped<IVartotojasRepository, VartotojasRepository>();
                        // services.AddScoped<IVartotojasService, VartotojasService>();
                        // services.AddScoped<IPasswordHasher, PasswordHasher>();
                        // services.AddScoped<IOrderRepository, OrderRepository>();
                        // services.AddScoped<IOrderService, OrderService>();
                    }
                );

            return host.Build();
        }
    }
}
