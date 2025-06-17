using HotelBookingSystem.InventoryService.Application.Interfaces;
using HotelBookingSystem.InventoryService.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace HotelBookingSystem.InventoryService.Infrastructure.Data
{
    public class InventoryDbContext : DbContext, IInventoryDbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<RoomInventory> RoomInventories { get; set; } = null!;
        public DbSet<RoomHold> RoomHolds { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply entity configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Add MassTransit inbox/outbox support for reliable messaging
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
        }
    }

    // Design-time factory for Entity Framework migrations
    public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
    {
        public InventoryDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../HotelBookingSystem.InventoryService.Api"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            var connectionString = configuration.GetConnectionString("inventorydb");
            
            optionsBuilder.UseNpgsql(connectionString);

            return new InventoryDbContext(optionsBuilder.Options);
        }
    }
} 