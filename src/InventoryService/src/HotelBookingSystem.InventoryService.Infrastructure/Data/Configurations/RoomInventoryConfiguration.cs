using HotelBookingSystem.InventoryService.Domain.Entities;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.InventoryService.Infrastructure.Data.Configurations;

public class RoomInventoryConfiguration : IEntityTypeConfiguration<RoomInventory>
{
    public void Configure(EntityTypeBuilder<RoomInventory> builder)
    {
        builder.ToTable("RoomInventories");
        
        // Primary key
        builder.HasKey(x => x.Id);
        
        // Value object conversions
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => InventoryId.From(value))
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
        
        // Basic properties
        builder.Property(x => x.Date)
            .HasColumnType("date")
            .IsRequired();
            
        builder.Property(x => x.TotalRooms)
            .IsRequired();
            
        builder.Property(x => x.AvailableRooms)
            .IsRequired();
            
        builder.Property(x => x.HeldRooms)
            .IsRequired();
            
        builder.Property(x => x.BookedRooms)
            .IsRequired();
            
        builder.Property(x => x.LastUpdated)
            .IsRequired();
        
        // Money value objects
        builder.OwnsOne(x => x.BasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BasePriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            money.Property(m => m.Currency)
                .HasColumnName("BasePriceCurrency")
                .HasConversion<string>()
                .IsRequired();
        });
        
        builder.OwnsOne(x => x.CurrentPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("CurrentPriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            money.Property(m => m.Currency)
                .HasColumnName("CurrentPriceCurrency")
                .HasConversion<string>()
                .IsRequired();
        });
        
        // Indexes for performance
        builder.HasIndex(x => new { x.HotelId, x.RoomTypeId, x.Date })
            .IsUnique()
            .HasDatabaseName("IX_RoomInventories_Hotel_RoomType_Date");
            
        builder.HasIndex(x => x.Date)
            .HasDatabaseName("IX_RoomInventories_Date");
            
        builder.HasIndex(x => new { x.HotelId, x.Date })
            .HasDatabaseName("IX_RoomInventories_Hotel_Date");
    }
} 