using KuroOpti.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuroOpti.Data
{
    public class KuroOptiDbContext : DbContext
    {
        public KuroOptiDbContext(DbContextOptions<KuroOptiDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FuelStation> FuelStations { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<SearchLog> SearchLogs { get; set; }
        public DbSet<UserRouteStation> UserRouteStations { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RoutePlanningHistory> RoutePlanningHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Email).IsUnique();
            });

            // FUEL STATION
            modelBuilder.Entity<FuelStation>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).HasMaxLength(255);
                entity.Property(x => x.Address).HasMaxLength(255);
                entity.Property(x => x.Municipality).HasMaxLength(255);

                entity.HasIndex(x => new { x.Name, x.Address }).IsUnique();

                entity.Property(x => x.Latitude).HasPrecision(9, 6);
                entity.Property(x => x.Longitude).HasPrecision(9, 6);

                entity.Property(x => x.DieselPrice).HasPrecision(6, 3);
                entity.Property(x => x.PetrolPrice).HasPrecision(6, 3);
                entity.Property(x => x.LpgPrice).HasPrecision(6, 3);
            });

            // ROUTE
            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.User).WithMany(u => u.Routes).HasForeignKey(x => x.UserId);

                entity.Property(x => x.StartLat).HasPrecision(9, 6);
                entity.Property(x => x.StartLng).HasPrecision(9, 6);
                entity.Property(x => x.EndLat).HasPrecision(9, 6);
                entity.Property(x => x.EndLng).HasPrecision(9, 6);
            });

            // SEARCH LOG
            modelBuilder.Entity<SearchLog>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity
                    .HasOne(x => x.User)
                    .WithMany(u => u.SearchLogs)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity
                    .HasOne(x => x.Route)
                    .WithMany(r => r.SearchLogs)
                    .HasForeignKey(x => x.RouteId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // USER ROUTE STATION
            modelBuilder.Entity<UserRouteStation>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity
                    .HasOne(x => x.Route)
                    .WithMany(r => r.SelectedStations)
                    .HasForeignKey(x => x.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(x => x.FuelStation)
                    .WithMany() // jei nereikia navigacijos iš FuelStation
                    .HasForeignKey(x => x.FuelStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
