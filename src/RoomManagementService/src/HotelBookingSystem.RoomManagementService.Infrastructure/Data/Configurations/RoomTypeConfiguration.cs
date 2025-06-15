using HotelBookingSystem.RoomManagementService.Domain.Entities;
using HotelBookingSystem.RoomManagementService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Configurations
{
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.ToTable("RoomTypes");

            // Primary Key
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id)
                .HasConversion(
                    id => id.Value,
                    value => RoomTypeId.From(value))
                .HasColumnName("Id");

            // Foreign Key
            builder.Property(rt => rt.HotelId)
                .HasConversion(
                    id => id.Value,
                    value => HotelId.From(value))
                .IsRequired();

            // Properties
            builder.Property(rt => rt.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(rt => rt.Description)
                .HasMaxLength(1000);

            builder.Property(rt => rt.MaxOccupancy)
                .IsRequired();

            builder.Property(rt => rt.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            builder.Property(rt => rt.UpdatedAt);

            // Complex Type - Money (BasePrice)
            builder.OwnsOne(rt => rt.BasePrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("BasePrice_Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("BasePrice_Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Complex Type - BedConfiguration
            builder.OwnsOne(rt => rt.BedConfiguration, bed =>
            {
                bed.Property(b => b.PrimaryBedType)
                    .HasColumnName("BedConfiguration_PrimaryBedType")
                    .HasConversion<string>()
                    .IsRequired();

                bed.Property(b => b.PrimaryBedCount)
                    .HasColumnName("BedConfiguration_PrimaryBedCount")
                    .IsRequired();

                bed.Property(b => b.SecondaryBedType)
                    .HasColumnName("BedConfiguration_SecondaryBedType")
                    .HasConversion<string>()
                    .IsRequired();

                bed.Property(b => b.SecondaryBedCount)
                    .HasColumnName("BedConfiguration_SecondaryBedCount")
                    .IsRequired();
            });

            // Complex Type - RoomFeatures
            builder.OwnsOne(rt => rt.Features, features =>
            {
                features.Property(f => f.Features)
                    .HasColumnName("Features")
                    .HasConversion(
                        list => string.Join(";", list.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)),
                        str => str.Split(';', StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.OrdinalIgnoreCase))
                    .HasMaxLength(2000)
                    .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyCollection<string>>(
                        (c1, c2) => new HashSet<string>(c1!, StringComparer.OrdinalIgnoreCase).SetEquals(c2!),
                        c => c.Select(v => StringComparer.OrdinalIgnoreCase.GetHashCode(v)).Aggregate(0, (a, v) => a ^ v),
                        c => c.ToHashSet(StringComparer.OrdinalIgnoreCase)));
            });

            // Indexes
            builder.HasIndex(rt => rt.HotelId)
                .HasDatabaseName("IX_RoomTypes_HotelId");

            builder.HasIndex(rt => new { rt.HotelId, rt.Name })
                .HasDatabaseName("IX_RoomTypes_HotelId_Name")
                .IsUnique();

            builder.HasIndex(rt => rt.Status)
                .HasDatabaseName("IX_RoomTypes_Status");

            // Ignore computed properties
            builder.Ignore(rt => rt.IsAvailable);
        }
    }
} 