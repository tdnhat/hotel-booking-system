using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.Events;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Entities;

public class RoomHold : Entity<HoldId>
{
    public Guid BookingId { get; private set; }
    public HotelId HotelId { get; private set; }
    public RoomTypeId RoomTypeId { get; private set; }
    public DateRange DateRange { get; private set; }
    public int RoomCount { get; private set; }
    public Money TotalAmount { get; private set; }
    public HoldStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ReleasedAt { get; private set; }
    public string? ReleaseReason { get; private set; }

    private RoomHold() { } // EF Core constructor

    public RoomHold(
        HoldId id,
        Guid bookingId,
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateRange dateRange,
        int roomCount,
        Money totalAmount,
        TimeSpan holdDuration)
    {
        if (roomCount <= 0)
            throw new ArgumentException("Room count must be positive");

        if (holdDuration <= TimeSpan.Zero)
            throw new ArgumentException("Hold duration must be positive");

        Id = id;
        BookingId = bookingId;
        HotelId = hotelId;
        RoomTypeId = roomTypeId;
        DateRange = dateRange;
        RoomCount = roomCount;
        TotalAmount = totalAmount;
        Status = HoldStatus.Active;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.Add(holdDuration);

        RaiseDomainEvent(new RoomHeldDomainEvent(
            Id, BookingId, HotelId, RoomTypeId, DateRange, RoomCount, TotalAmount, ExpiresAt));
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt && Status == HoldStatus.Active;

    public bool IsActive => Status == HoldStatus.Active && !IsExpired;

    public bool CanBeConfirmed => Status == HoldStatus.Active && !IsExpired;

    public bool CanBeReleased => Status == HoldStatus.Active;

    public void Confirm()
    {
        if (!CanBeConfirmed)
            throw new InvalidOperationException("Hold cannot be confirmed");

        Status = HoldStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        RaiseDomainEvent(new BookingConfirmedDomainEvent(
            Id, BookingId, HotelId, RoomTypeId, DateRange, RoomCount, TotalAmount));
    }

    public void Release(string reason)
    {
        if (!CanBeReleased)
            throw new InvalidOperationException("Hold cannot be released");

        Status = HoldStatus.Released;
        ReleasedAt = DateTime.UtcNow;
        ReleaseReason = reason;

        RaiseDomainEvent(new RoomReleasedDomainEvent(Id, BookingId, RoomCount, reason));
    }

    public void MarkAsExpired()
    {
        if (Status != HoldStatus.Active)
            return;

        Status = HoldStatus.Expired;
        ReleasedAt = DateTime.UtcNow;
        ReleaseReason = "Expired";

        RaiseDomainEvent(new RoomReleasedDomainEvent(Id, BookingId, RoomCount, "Expired"));
    }

    public TimeSpan GetRemainingTime()
    {
        if (Status != HoldStatus.Active)
            return TimeSpan.Zero;

        var remaining = ExpiresAt - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
} 