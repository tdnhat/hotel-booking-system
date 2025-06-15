using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("Hotels");

            // Primary Key
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id)
                .HasConversion(
                    id => id.Value,
                    value => HotelId.From(value))
                .HasColumnName("Id");

            // Properties
            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Description)
                .HasMaxLength(2000);

            builder.Property(h => h.ContactEmail)
                .HasMaxLength(255);

            builder.Property(h => h.ContactPhone)
                .HasMaxLength(50);

            builder.Property(h => h.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(h => h.CreatedAt)
                .IsRequired();

            builder.Property(h => h.UpdatedAt);

            // Complex Type - Address
            builder.OwnsOne(h => h.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("Address_Street")
                    .HasMaxLength(255)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("Address_City")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.State)
                    .HasColumnName("Address_State")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("Address_Country")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.PostalCode)
                    .HasColumnName("Address_PostalCode")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            // Note: Index on address columns will be added in a future migration
            // after the columns are created

            // StarRating (enum)
            builder.Property(h => h.StarRating)
                .HasConversion<string>()
                .IsRequired();

            // Relationships
            builder.HasMany(h => h.RoomTypes)
                .WithOne()
                .HasForeignKey(rt => rt.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(h => h.Name)
                .HasDatabaseName("IX_Hotels_Name");

            builder.HasIndex(h => h.Status)
                .HasDatabaseName("IX_Hotels_Status");

            // Ignore navigation properties that are computed
            builder.Ignore(h => h.IsOperational);
            builder.Ignore(h => h.HasAvailableRoomTypes);
            builder.Ignore(h => h.TotalRoomTypeCount);
            builder.Ignore(h => h.ActiveRoomTypeCount);
        }
    }
} 