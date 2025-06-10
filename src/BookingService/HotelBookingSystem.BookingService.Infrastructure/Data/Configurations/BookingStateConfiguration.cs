using HotelBookingSystem.BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.BookingService.Infrastructure.Data.Configurations
{
    public class BookingStateConfiguration : IEntityTypeConfiguration<BookingState>
    {
        public void Configure(EntityTypeBuilder<BookingState> builder)
        {
            // Primary key is CorrelationId (this is important for MassTransit Saga)
            builder.HasKey(bs => bs.CorrelationId);

            // Properties
            builder.Property(bs => bs.BookingId)
                .IsRequired();

            builder.Property(bs => bs.RoomTypeId)
                .IsRequired();

            builder.Property(bs => bs.HotelId)
                .IsRequired();

            builder.Property(bs => bs.GuestEmail)
                .HasMaxLength(254)
                .IsRequired();

            builder.Property(bs => bs.TotalPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(bs => bs.CheckInDate)
                .IsRequired();

            builder.Property(bs => bs.CheckOutDate)
                .IsRequired();

            builder.Property(bs => bs.NumberOfGuests)
                .IsRequired();

            builder.Property(bs => bs.CreatedAt)
                .IsRequired();

            builder.Property(bs => bs.UpdatedAt)
                .IsRequired();

            builder.Property(bs => bs.FailureReason)
                .HasMaxLength(500);

            builder.Property(bs => bs.RoomHoldReference)
                .HasMaxLength(100);

            builder.Property(bs => bs.PaymentReference)
                .HasMaxLength(100);

            // Enum conversion
            builder.Property(bs => bs.CurrentState)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            // Concurrency token for optimistic locking
            builder.Property(bs => bs.Version)
                .IsConcurrencyToken()
                .IsRequired(false)
                .HasDefaultValue(new byte[0]);

            // Indexes for performance
            builder.HasIndex(bs => bs.BookingId)
                .IsUnique();

            builder.HasIndex(bs => bs.GuestEmail);

            builder.HasIndex(bs => bs.CurrentState);

            builder.HasIndex(bs => bs.CreatedAt);

            // Table name
            builder.ToTable("BookingStates");
        }

    }
}