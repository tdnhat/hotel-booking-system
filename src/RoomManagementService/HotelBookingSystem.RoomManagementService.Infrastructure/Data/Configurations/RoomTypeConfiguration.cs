using HotelBookingSystem.RoomManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Configurations
{
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
            builder.Property(rt => rt.Description).HasMaxLength(500);
            builder.Property(rt => rt.PricePerNight).HasPrecision(18, 2);

            // Configure one-to-many relationship
            builder.HasMany(rt => rt.Rooms)
                  .WithOne()
                  .HasForeignKey(r => r.RoomTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}