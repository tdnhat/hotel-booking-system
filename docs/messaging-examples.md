# 🔄 Messaging Examples - Quick Reference

## 📨 Current Shared Contracts (✅ **Implemented**)

> **Located in**: `src/Shared/HotelBookingSystem.Contracts/`

### 🏨 **Commands** (Booking Service → Other Services)

```csharp
// HotelBookingSystem.Contracts.Commands.HoldRoom
public class HoldRoom
{
    public Guid BookingId { get; set; }
    public Guid RoomTypeId { get; set; }
    public Guid HotelId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public TimeSpan HoldDuration { get; set; } = TimeSpan.FromMinutes(10);
}

// HotelBookingSystem.Contracts.Commands.ProcessPayment
public class ProcessPayment
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethod { get; set; } = string.Empty;
    public string RoomHoldReference { get; set; } = string.Empty;
}

// HotelBookingSystem.Contracts.Commands.ReleaseRoom
public class ReleaseRoom
{
    public Guid BookingId { get; set; }
    public string RoomHoldReference { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
}
```

### 📥 **Events** (Other Services → Booking Service)

```csharp
// HotelBookingSystem.Contracts.Events.RoomHeld
public class RoomHeld
{
    public Guid BookingId { get; set; }
    public string RoomHoldReference { get; set; } = string.Empty;
    public DateTime HeldUntil { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
}

// HotelBookingSystem.Contracts.Events.RoomHoldFailed
public class RoomHoldFailed
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}

// HotelBookingSystem.Contracts.Events.PaymentSucceeded
public class PaymentSucceeded
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}

// HotelBookingSystem.Contracts.Events.PaymentFailed
public class PaymentFailed
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}

// HotelBookingSystem.Contracts.Events.RoomReleased
public class RoomReleased
{
    public Guid BookingId { get; set; }
    public string RoomHoldReference { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
}
```

## 🔄 **Booking Saga Flow** (✅ **Implemented**)

Based on the actual `BookingSagaStateMachine.cs`:

```
1. 🚀 User → Booking API: Create booking request
2. 📤 BookingSaga: Publishes HoldRoom command
3. 📥 Inventory Service: Responds with RoomHeld/RoomHoldFailed
4. 📤 BookingSaga: Publishes ProcessPayment command (if room held)
5. 📥 Payment Service: Responds with PaymentSucceeded/PaymentFailed
6. 📤 BookingSaga: Publishes ReleaseRoom (if payment failed)
7. ✅ Final State: Confirmed or Failed
```

## 🔧 **Additional Contracts Needed for Other Services**

### For RoomManagement ↔ Inventory Communication

```csharp
// Future contracts (not yet implemented)
public class RoomTypeCreated
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; }
    public int MaxOccupancy { get; set; }
}

public class RoomAvailabilityUpdated
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalRooms { get; set; }
    public decimal CurrentPrice { get; set; }
}
```

### For Booking ↔ All Services Notifications

```csharp
// Future contracts (not yet implemented)  
public class BookingConfirmed
{
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalAmount { get; set; }
    public string TransactionId { get; set; }
    public DateTime BookedAt { get; set; }
}
```

## 🔧 **Consumer Examples for Current Contracts**

### Inventory Service Consumers (To Implement)

```csharp
// Consumes: HoldRoom command
public class HoldRoomConsumer : IConsumer<HoldRoom>
{
    public async Task Consume(ConsumeContext<HoldRoom> context)
    {
        var command = context.Message;
        
        try
        {
            // Check availability and create hold
            var holdResult = await _inventoryService.HoldRoomAsync(
                command.HotelId, 
                command.RoomTypeId,
                command.CheckInDate, 
                command.CheckOutDate,
                command.NumberOfGuests,
                command.HoldDuration);
                
            // Respond with success
            await context.Publish(new RoomHeld
            {
                BookingId = command.BookingId,
                RoomHoldReference = holdResult.HoldReference,
                HeldUntil = holdResult.HeldUntil,
                RoomNumber = holdResult.RoomNumber
            });
        }
        catch (InsufficientInventoryException ex)
        {
            // Respond with failure
            await context.Publish(new RoomHoldFailed
            {
                BookingId = command.BookingId,
                Reason = ex.Message,
                FailedAt = DateTime.UtcNow
            });
        }
    }
}

// Consumes: ReleaseRoom command
public class ReleaseRoomConsumer : IConsumer<ReleaseRoom>
{
    public async Task Consume(ConsumeContext<ReleaseRoom> context)
    {
        var command = context.Message;
        
        await _inventoryService.ReleaseHoldAsync(command.RoomHoldReference);
        
        await context.Publish(new RoomReleased
        {
            BookingId = command.BookingId,
            RoomHoldReference = command.RoomHoldReference,
            ReleasedAt = DateTime.UtcNow
        });
    }
}
```

### Payment Service Consumers (To Implement)

```csharp
// Consumes: ProcessPayment command
public class ProcessPaymentConsumer : IConsumer<ProcessPayment>
{
    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        var command = context.Message;
        
        try
        {
            var result = await _paymentService.ProcessPaymentAsync(
                command.BookingId,
                command.Amount,
                command.Currency,
                command.PaymentMethod);
                
            await context.Publish(new PaymentSucceeded
            {
                BookingId = command.BookingId,
                Amount = command.Amount,
                Currency = command.Currency,
                TransactionId = result.TransactionId,
                ProcessedAt = DateTime.UtcNow
            });
        }
        catch (PaymentFailedException ex)
        {
            await context.Publish(new PaymentFailed
            {
                BookingId = command.BookingId,
                Amount = command.Amount,
                Currency = command.Currency,
                Reason = ex.Message,
                FailedAt = DateTime.UtcNow
            });
        }
    }
}
```

## ⚙️ **MassTransit Configuration**

Each service references the shared contracts:

```xml
<!-- In each service's .csproj -->
<ProjectReference Include="..\..\Shared\HotelBookingSystem.Contracts\HotelBookingSystem.Contracts.csproj" />
```

```csharp
// MassTransit configuration
services.AddMassTransit(config =>
{
    config.AddConsumer<YourConsumer>();
    
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(connectionString);
        cfg.ConfigureEndpoints(context);
    });
});
```

## 📋 **Implementation Status**

- ✅ **Shared Contracts**: Core booking flow contracts implemented
- ✅ **Booking Service**: Complete with saga orchestration
- ⏳ **Inventory Service**: Needs to implement consumers for HoldRoom/ReleaseRoom
- ⏳ **Payment Service**: Needs to implement consumer for ProcessPayment
- ⏳ **RoomManagement Service**: Needs additional contracts for room type management

## 🚀 **Next Steps**

1. **Clean up old domain messages** in BookingService.Domain.Messages (if no longer used)
2. **Add missing contracts** to Shared folder for inter-service communication
3. **Implement Inventory Service** with consumers for current contracts
4. **Implement Payment Service** with consumers for current contracts
5. **Add RoomManagement integration** with new contracts for room type management

## 🔍 **Key Difference from Previous Documentation**

The actual implementation uses **simpler, focused contracts** compared to my earlier documentation. The BookingSaga handles the orchestration using these lightweight messages, making the system easier to understand and maintain. 