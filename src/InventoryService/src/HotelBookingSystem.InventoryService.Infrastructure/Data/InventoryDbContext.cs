using HotelBookingSystem.InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.InventoryService.Infrastructure.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<RoomAvailability> RoomAvailabilities { get; set; }
        public DbSet<RoomHold> RoomHolds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoomAvailability>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoomTypeId).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Create unique index for RoomTypeId + Date combination
                entity.HasIndex(e => new { e.RoomTypeId, e.Date }).IsUnique();
            });

            modelBuilder.Entity<RoomHold>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HoldReference).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BookingId).IsRequired();
                entity.Property(e => e.RoomTypeId).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Create unique index for HoldReference
                entity.HasIndex(e => e.HoldReference).IsUnique();
                
                // Create index for BookingId lookups
                entity.HasIndex(e => e.BookingId);
            });
        }
    }
} 