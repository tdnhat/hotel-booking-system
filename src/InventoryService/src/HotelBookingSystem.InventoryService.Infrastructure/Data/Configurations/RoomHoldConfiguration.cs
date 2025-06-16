using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.InventoryService.Infrastructure.Data.Configurations;

public class RoomHoldConfiguration : IEntityTypeConfiguration<RoomHold>
{
    public void Configure(EntityTypeBuilder<RoomHold> builder)
    {
        builder.ToTable("RoomHolds");
        
        // Primary key
        builder.HasKey(x => x.Id);
        
        // Value object conversions
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => HoldId.From(value))
            .IsRequired();
            
        builder.Property(x => x.HotelId)
            .HasConversion(
                id => id.Value,
                value => HotelId.From(value))
            .IsRequired();
            
        builder.Property(x => x.RoomTypeId)
            .HasConversion(
                id => id.Value,
                value => RoomTypeId.From(value))
            .IsRequired();
            
        builder.Property(x => x.BookingId)
            .IsRequired();
        
        // DateRange value object
        builder.OwnsOne(x => x.DateRange, dateRange =>
        {
            dateRange.Property(dr => dr.CheckIn)
                .HasColumnName("CheckInDate")
                .HasColumnType("date")
                .IsRequired();
                
            dateRange.Property(dr => dr.CheckOut)
                .HasColumnName("CheckOutDate")
                .HasColumnType("date")
                .IsRequired();
        });
        
        // Money value object
        builder.OwnsOne(x => x.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
        });
        
        // Other properties
        builder.Property(x => x.RoomCount)
            .IsRequired();
            
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();
            
        builder.Property(x => x.ExpiresAt)
            .IsRequired();
            
        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        builder.Property(x => x.HoldReference)
            .HasMaxLength(100)
            .IsRequired();
        
        // Indexes for performance
        builder.HasIndex(x => x.BookingId)
            .HasDatabaseName("IX_RoomHolds_BookingId");
            
        builder.HasIndex(x => x.HoldReference)
            .IsUnique()
            .HasDatabaseName("IX_RoomHolds_HoldReference");
            
        builder.HasIndex(x => new { x.HotelId, x.RoomTypeId })
            .HasDatabaseName("IX_RoomHolds_Hotel_RoomType");
            
        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_RoomHolds_ExpiresAt");
            
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_RoomHolds_Status");
    }
} 