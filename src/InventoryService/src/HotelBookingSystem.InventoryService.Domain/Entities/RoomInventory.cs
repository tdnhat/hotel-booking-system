using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.InventoryService.Domain.Enums;
using HotelBookingSystem.InventoryService.Domain.Events;
using HotelBookingSystem.InventoryService.Domain.ValueObjects;

namespace HotelBookingSystem.InventoryService.Domain.Entities;

public class RoomInventory : Entity<InventoryId>
{
    public HotelId HotelId { get; private set; }
    public RoomTypeId RoomTypeId { get; private set; }
    public DateTime Date { get; private set; }
    public int TotalRooms { get; private set; }
    public int AvailableRooms { get; private set; }
    public int HeldRooms { get; private set; }
    public int BookedRooms { get; private set; }
    public Money BasePrice { get; private set; }
    public Money CurrentPrice { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private RoomInventory() 
    { 
        // EF Core constructor - properties will be set by EF Core
        HotelId = null!;
        RoomTypeId = null!;
        BasePrice = null!;
        CurrentPrice = null!;
    }

    public RoomInventory(
        InventoryId id,
        HotelId hotelId,
        RoomTypeId roomTypeId,
        DateTime date,
        int totalRooms,
        Money basePrice)
    {
        if (totalRooms <= 0)
            throw new ArgumentException("Total rooms must be greater than zero");

        Id = id;
        HotelId = hotelId;
        RoomTypeId = roomTypeId;
        Date = date.Date;
        TotalRooms = totalRooms;
        AvailableRooms = totalRooms;
        HeldRooms = 0;
        BookedRooms = 0;        BasePrice = basePrice;
        CurrentPrice = new Money(basePrice.Amount, basePrice.Currency); // Create separate instance
        LastUpdated = DateTime.UtcNow;
    }

    public bool CanHold(int roomCount)
    {
        return AvailableRooms >= roomCount && roomCount > 0;
    }

    public void HoldRooms(int roomCount)
    {
        if (!CanHold(roomCount))
            throw new InvalidOperationException($"Cannot hold {roomCount} rooms. Only {AvailableRooms} available.");

        AvailableRooms -= roomCount;
        HeldRooms += roomCount;
        LastUpdated = DateTime.UtcNow;

        RaiseDomainEvent(new RoomAvailabilityUpdatedDomainEvent(
            HotelId, RoomTypeId, Date, AvailableRooms, TotalRooms, CurrentPrice));
    }

    public void ReleaseHold(int roomCount)
    {
        if (roomCount <= 0)
            throw new ArgumentException("Room count must be positive");

        if (roomCount > HeldRooms)
            throw new InvalidOperationException($"Cannot release {roomCount} held rooms. Only {HeldRooms} are held.");

        HeldRooms -= roomCount;
        AvailableRooms += roomCount;
        LastUpdated = DateTime.UtcNow;

        RaiseDomainEvent(new RoomAvailabilityUpdatedDomainEvent(
            HotelId, RoomTypeId, Date, AvailableRooms, TotalRooms, CurrentPrice));
    }

    public void ConfirmBooking(int roomCount)
    {
        if (roomCount <= 0)
            throw new ArgumentException("Room count must be positive");

        if (roomCount > HeldRooms)
            throw new InvalidOperationException($"Cannot confirm {roomCount} rooms. Only {HeldRooms} are held.");

        HeldRooms -= roomCount;
        BookedRooms += roomCount;
        LastUpdated = DateTime.UtcNow;

        RaiseDomainEvent(new RoomAvailabilityUpdatedDomainEvent(
            HotelId, RoomTypeId, Date, AvailableRooms, TotalRooms, CurrentPrice));
    }

    public void UpdatePricing(Money newPrice)
    {
        if (newPrice.Currency != CurrentPrice.Currency)
            throw new InvalidOperationException("Cannot change currency");

        CurrentPrice = newPrice;
        LastUpdated = DateTime.UtcNow;

        RaiseDomainEvent(new RoomAvailabilityUpdatedDomainEvent(
            HotelId, RoomTypeId, Date, AvailableRooms, TotalRooms, CurrentPrice));
    }

    public void AddRooms(int roomCount)
    {
        if (roomCount <= 0)
            throw new ArgumentException("Room count must be positive");

        TotalRooms += roomCount;
        AvailableRooms += roomCount;
        LastUpdated = DateTime.UtcNow;

        RaiseDomainEvent(new RoomAvailabilityUpdatedDomainEvent(
            HotelId, RoomTypeId, Date, AvailableRooms, TotalRooms, CurrentPrice));
    }

    public void RemoveRooms(int roomCount)
    {
        if (roomCount <= 0)
            throw new ArgumentException("Room count must be positive");

        if (roomCount > AvailableRooms)
            throw new InvalidOperationException($"Cannot remove {roomCount} rooms. Only {AvailableRooms} available.");

        TotalRooms -= roomCount;
        AvailableRooms -= roomCount;
        LastUpdated = DateTime.UtcNow;

        RaiseDomainEvent(new RoomAvailabilityUpdatedDomainEvent(
            HotelId, RoomTypeId, Date, AvailableRooms, TotalRooms, CurrentPrice));
    }

    public decimal GetOccupancyRate()
    {
        if (TotalRooms == 0) return 0;
        return (decimal)(BookedRooms + HeldRooms) / TotalRooms;
    }

    public bool IsFullyBooked => AvailableRooms == 0 && HeldRooms == 0;
    
    public bool HasAvailability => AvailableRooms > 0;
} 