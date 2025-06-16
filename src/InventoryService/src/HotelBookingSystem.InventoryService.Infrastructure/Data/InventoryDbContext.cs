using HotelBookingSystem.InventoryService.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
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

    public interface IInventoryDbContext
    {
        DbSet<RoomInventory> RoomInventories { get; }
        DbSet<RoomHold> RoomHolds { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 