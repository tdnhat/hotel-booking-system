using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data
{
    public class RoomManagementDbContextFactory : IDesignTimeDbContextFactory<RoomManagementDbContext>
    {
        public RoomManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RoomManagementDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=roommanagement;Username=postgres;Password=postgres"); // Replace with your actual connection string

            return new RoomManagementDbContext(optionsBuilder.Options);
        }
    }
}