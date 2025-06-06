using HotelBookingSystem.RoomManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {

        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
            builder.Property(r => r.Status).HasConversion<string>();
        }
    }
}