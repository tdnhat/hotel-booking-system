using System.Reflection;
using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data
{
    public class RoomManagementDbContext : DbContext, IRoomManagementDbContext
    {
        public RoomManagementDbContext(DbContextOptions<RoomManagementDbContext> options) : base(options)
        {
        }

        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<Room> Rooms => Set<Room>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            // Enable sensitive data logging in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
            }
        }
    }
}
