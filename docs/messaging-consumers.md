# 🔧 Messaging Consumers Implementation Guide

## 📋 Overview

This document provides concrete examples of message consumers for each service in the Hotel Booking System. Use these as reference implementations when building the remaining services.

## 🏨 RoomManagement Service Consumers

### 📥 BookingConfirmedConsumer

```csharp
public class BookingConfirmedConsumer : IConsumer<BookingConfirmedEvent>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ILogger<BookingConfirmedConsumer> _logger;

    public BookingConfirmedConsumer(
        IHotelRepository hotelRepository,
        ILogger<BookingConfirmedConsumer> logger)
    {
        _hotelRepository = hotelRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingConfirmedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing booking confirmation for BookingId: {BookingId}", message.BookingId);

        try
        {
            // Update room type booking statistics
            var hotel = await _hotelRepository.GetByIdWithRoomTypesAsync(HotelId.From(message.HotelId));
            if (hotel != null)
            {
                // Could implement booking statistics tracking here
                // Example: Track total bookings, revenue, popular room types
                _logger.LogInformation("Updated booking statistics for Hotel: {HotelId}, RoomType: {RoomTypeId}", 
                    message.HotelId, message.RoomTypeId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing booking confirmation for BookingId: {BookingId}", message.BookingId);
            throw; // Will trigger retry mechanism
        }
    }
}
```

### 📥 RoomAvailabilityUpdatedConsumer (Dynamic Pricing)

```csharp
public class RoomAvailabilityUpdatedConsumer : IConsumer<RoomAvailabilityUpdatedEvent>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RoomAvailabilityUpdatedConsumer> _logger;

    public RoomAvailabilityUpdatedConsumer(
        IHotelRepository hotelRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<RoomAvailabilityUpdatedConsumer> logger)
    {
        _hotelRepository = hotelRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RoomAvailabilityUpdatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing availability update for RoomType: {RoomTypeId}, Available: {Available}/{Total}", 
            message.RoomTypeId, message.AvailableRooms, message.TotalRooms);

        try
        {
            var hotel = await _hotelRepository.GetByIdWithRoomTypesAsync(HotelId.From(message.HotelId));
            if (hotel == null) return;

            var roomType = hotel.GetRoomType(RoomTypeId.From(message.RoomTypeId));
            
            // Calculate occupancy rate
            var occupancyRate = (double)(message.TotalRooms - message.AvailableRooms) / message.TotalRooms;
            
            // Dynamic pricing logic
            Money newPrice = null;
            string reason = null;
            
            if (occupancyRate > 0.8) // High demand - increase price by 20%
            {
                newPrice = Money.Create(message.CurrentPrice * 1.2m, "USD");
                reason = "DynamicPricing - High Demand";
            }
            else if (occupancyRate < 0.3) // Low demand - decrease price by 10%
            {
                newPrice = Money.Create(message.CurrentPrice * 0.9m, "USD");
                reason = "DynamicPricing - Low Demand";
            }
            
            if (newPrice != null)
            {
                roomType.UpdatePrice(newPrice);
                await _hotelRepository.UpdateAsync(hotel);
                
                await _publishEndpoint.Publish(new RoomTypePriceUpdatedEvent
                {
                    HotelId = message.HotelId,
                    RoomTypeId = message.RoomTypeId,
                    NewPrice = newPrice.Amount,
                    OldPrice = message.CurrentPrice,
                    Currency = newPrice.Currency,
                    EffectiveDate = message.Date,
                    Reason = reason
                });
                
                _logger.LogInformation("Updated price for RoomType: {RoomTypeId} from {OldPrice} to {NewPrice} ({Reason})", 
                    message.RoomTypeId, message.CurrentPrice, newPrice.Amount, reason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing availability update for RoomType: {RoomTypeId}", message.RoomTypeId);
            throw;
        }
    }
}
```

### ⚙️ RoomManagement MassTransit Configuration

```csharp
// Add to RoomManagement.Infrastructure/DependencyInjection.cs
services.AddMassTransit(config =>
{
    // Register consumers
    config.AddConsumer<BookingConfirmedConsumer>();
    config.AddConsumer<RoomAvailabilityUpdatedConsumer>();
    
    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = configuration.GetConnectionString("messaging");
        if (!string.IsNullOrEmpty(connectionString))
        {
            cfg.Host(connectionString);
        }
        else
        {
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            cfg.Host(rabbitConfig["Host"], h =>
            {
                h.Username(rabbitConfig["Username"]);
                h.Password(rabbitConfig["Password"]);
            });
        }

        cfg.ConfigureEndpoints(context);
    });
});
```

---

## 📦 Inventory Service Consumers

### 📥 RoomTypeCreatedConsumer

```csharp
public class RoomTypeCreatedConsumer : IConsumer<RoomTypeCreatedEvent>
{
    private readonly IRoomInventoryService _inventoryService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RoomTypeCreatedConsumer> _logger;

    public RoomTypeCreatedConsumer(
        IRoomInventoryService inventoryService,
        IPublishEndpoint publishEndpoint,
        ILogger<RoomTypeCreatedConsumer> logger)
    {
        _inventoryService = inventoryService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RoomTypeCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Creating inventory for new RoomType: {RoomTypeId} at Hotel: {HotelId}", 
            message.RoomTypeId, message.HotelId);

        try
        {
            // Initialize inventory for the next 365 days with default room count
            await _inventoryService.InitializeRoomTypeInventoryAsync(
                message.HotelId,
                message.RoomTypeId,
                message.BasePrice,
                defaultRoomCount: 10, // Default room count per room type
                daysAhead: 365);
                
            // Publish initial availability
            await _publishEndpoint.Publish(new RoomAvailabilityUpdatedEvent
            {
                HotelId = message.HotelId,
                RoomTypeId = message.RoomTypeId,
                Date = DateTime.Today,
                AvailableRooms = 10,
                TotalRooms = 10,
                CurrentPrice = message.BasePrice,
                UpdatedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Successfully initialized inventory for RoomType: {RoomTypeId}", message.RoomTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing inventory for RoomType: {RoomTypeId}", message.RoomTypeId);
            throw;
        }
    }
}
```

### 📥 HoldRoomConsumer

```csharp
public class HoldRoomConsumer : IConsumer<HoldRoomCommand>
{
    private readonly IRoomInventoryService _inventoryService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<HoldRoomConsumer> _logger;

    public HoldRoomConsumer(
        IRoomInventoryService inventoryService,
        IPublishEndpoint publishEndpoint,
        ILogger<HoldRoomConsumer> logger)
    {
        _inventoryService = inventoryService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<HoldRoomCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Processing room hold request for BookingId: {BookingId}, Rooms: {RoomCount}", 
            command.BookingId, command.RoomCount);

        try
        {
            // Check availability first
            var availability = await _inventoryService.CheckAvailabilityAsync(
                command.HotelId,
                command.RoomTypeId,
                command.CheckIn,
                command.CheckOut);
                
            if (availability.AvailableRooms < command.RoomCount)
            {
                await _publishEndpoint.Publish(new RoomHoldFailedEvent
                {
                    BookingId = command.BookingId,
                    HotelId = command.HotelId,
                    RoomTypeId = command.RoomTypeId,
                    CheckIn = command.CheckIn,
                    CheckOut = command.CheckOut,
                    RequestedRoomCount = command.RoomCount,
                    AvailableRoomCount = availability.AvailableRooms,
                    Reason = $"Insufficient inventory. Requested: {command.RoomCount}, Available: {availability.AvailableRooms}",
                    FailedAt = DateTime.UtcNow
                });
                
                _logger.LogWarning("Insufficient inventory for BookingId: {BookingId}. Requested: {RequestedRooms}, Available: {AvailableRooms}", 
                    command.BookingId, command.RoomCount, availability.AvailableRooms);
                return;
            }

            // Create hold
            var hold = await _inventoryService.HoldRoomsAsync(
                command.HotelId,
                command.RoomTypeId,
                command.CheckIn,
                command.CheckOut,
                command.RoomCount,
                command.HoldDuration);
                
            await _publishEndpoint.Publish(new RoomHeldEvent
            {
                BookingId = command.BookingId,
                HoldId = hold.Id,
                HotelId = command.HotelId,
                RoomTypeId = command.RoomTypeId,
                CheckIn = command.CheckIn,
                CheckOut = command.CheckOut,
                RoomCount = command.RoomCount,
                TotalAmount = hold.TotalAmount,
                ExpiresAt = hold.ExpiresAt,
                HeldAt = DateTime.UtcNow
            });
            
            // Publish updated availability
            var updatedAvailability = await _inventoryService.CheckAvailabilityAsync(
                command.HotelId,
                command.RoomTypeId,
                command.CheckIn,
                command.CheckOut);
                
            await _publishEndpoint.Publish(new RoomAvailabilityUpdatedEvent
            {
                HotelId = command.HotelId,
                RoomTypeId = command.RoomTypeId,
                Date = command.CheckIn,
                AvailableRooms = updatedAvailability.AvailableRooms,
                TotalRooms = updatedAvailability.TotalRooms,
                CurrentPrice = updatedAvailability.CurrentPrice,
                UpdatedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Successfully held {RoomCount} rooms for BookingId: {BookingId}, HoldId: {HoldId}", 
                command.RoomCount, command.BookingId, hold.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing room hold for BookingId: {BookingId}", command.BookingId);
            throw;
        }
    }
}
```

### 📥 ReleaseRoomHoldConsumer

```csharp
public class ReleaseRoomHoldConsumer : IConsumer<ReleaseRoomHoldCommand>
{
    private readonly IRoomInventoryService _inventoryService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ReleaseRoomHoldConsumer> _logger;

    public async Task Consume(ConsumeContext<ReleaseRoomHoldCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Releasing room hold for BookingId: {BookingId}, HoldId: {HoldId}", 
            command.BookingId, command.HoldId);

        try
        {
            var holdDetails = await _inventoryService.GetHoldDetailsAsync(command.HoldId);
            var releasedCount = await _inventoryService.ReleaseHoldAsync(command.HoldId, command.Reason);
            
            await _publishEndpoint.Publish(new RoomReleasedEvent
            {
                BookingId = command.BookingId,
                HoldId = command.HoldId,
                ReleasedRoomCount = releasedCount,
                Reason = command.Reason,
                ReleasedAt = DateTime.UtcNow
            });
            
            // Publish updated availability
            if (holdDetails != null)
            {
                var updatedAvailability = await _inventoryService.CheckAvailabilityAsync(
                    holdDetails.HotelId,
                    holdDetails.RoomTypeId,
                    holdDetails.CheckIn,
                    holdDetails.CheckOut);
                    
                await _publishEndpoint.Publish(new RoomAvailabilityUpdatedEvent
                {
                    HotelId = holdDetails.HotelId,
                    RoomTypeId = holdDetails.RoomTypeId,
                    Date = holdDetails.CheckIn,
                    AvailableRooms = updatedAvailability.AvailableRooms,
                    TotalRooms = updatedAvailability.TotalRooms,
                    CurrentPrice = updatedAvailability.CurrentPrice,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            
            _logger.LogInformation("Released {RoomCount} rooms for BookingId: {BookingId}", 
                releasedCount, command.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing room hold for BookingId: {BookingId}", command.BookingId);
            throw;
        }
    }
}
```

---

## 💳 Payment Service Consumers

### 📥 ProcessPaymentConsumer

```csharp
public class ProcessPaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    private readonly IPaymentService _paymentService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ProcessPaymentConsumer> _logger;

    public ProcessPaymentConsumer(
        IPaymentService paymentService,
        IPublishEndpoint publishEndpoint,
        ILogger<ProcessPaymentConsumer> logger)
    {
        _paymentService = paymentService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Processing payment for BookingId: {BookingId}, Amount: {Amount} {Currency}", 
            command.BookingId, command.Amount, command.Currency);

        try
        {
            // Process payment through payment gateway
            var result = await _paymentService.ProcessPaymentAsync(
                command.PaymentId,
                command.Amount,
                command.Currency,
                command.PaymentMethod,
                command.PaymentDetails);
                
            await _publishEndpoint.Publish(new PaymentProcessedEvent
            {
                BookingId = command.BookingId,
                PaymentId = command.PaymentId,
                Success = true,
                Amount = command.Amount,
                Currency = command.Currency,
                PaymentMethod = command.PaymentMethod,
                TransactionId = result.TransactionId,
                ProcessedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Payment successful for BookingId: {BookingId}, TransactionId: {TransactionId}", 
                command.BookingId, result.TransactionId);
        }
        catch (InsufficientFundsException ex)
        {
            await _publishEndpoint.Publish(new PaymentProcessedEvent
            {
                BookingId = command.BookingId,
                PaymentId = command.PaymentId,
                Success = false,
                Amount = command.Amount,
                Currency = command.Currency,
                PaymentMethod = command.PaymentMethod,
                FailureReason = "Insufficient funds",
                ProcessedAt = DateTime.UtcNow
            });
            
            _logger.LogWarning("Payment failed for BookingId: {BookingId} - Insufficient funds", command.BookingId);
        }
        catch (PaymentDeclinedException ex)
        {
            await _publishEndpoint.Publish(new PaymentProcessedEvent
            {
                BookingId = command.BookingId,
                PaymentId = command.PaymentId,
                Success = false,
                Amount = command.Amount,
                Currency = command.Currency,
                PaymentMethod = command.PaymentMethod,
                FailureReason = ex.DeclineReason,
                ProcessedAt = DateTime.UtcNow
            });
            
            _logger.LogWarning("Payment declined for BookingId: {BookingId} - {Reason}", 
                command.BookingId, ex.DeclineReason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for BookingId: {BookingId}", command.BookingId);
            throw;
        }
    }
}
```

---

## 📋 Implementation Checklist

### ✅ Completed
- [x] Booking Service (with Saga pattern)
- [x] RoomManagement Service Infrastructure
- [x] Consumer examples and patterns

### ⏳ Pending Implementation
- [ ] **RoomManagement Service Consumers**
  - [ ] Add BookingConfirmedConsumer
  - [ ] Add RoomAvailabilityUpdatedConsumer
  - [ ] Configure MassTransit registration

- [ ] **Inventory Service**
  - [ ] Create domain models (RoomInventory, RoomHold)
  - [ ] Implement IRoomInventoryService
  - [ ] Add all consumers (RoomTypeCreated, HoldRoom, ReleaseHold)
  - [ ] Create database schema and configurations

- [ ] **Payment Service**
  - [ ] Create domain models (Payment, Refund)
  - [ ] Implement IPaymentService with gateway integration
  - [ ] Add consumers (ProcessPayment, ProcessRefund)
  - [ ] Create database schema and configurations

### 🔧 Configuration Steps
1. **Add Consumer Registration**: Update each service's DependencyInjection.cs
2. **Database Migrations**: Create EF migrations for new entities
3. **Message Contracts**: Create shared message contracts library
4. **Testing**: Create integration tests for message flows
5. **Monitoring**: Add logging and health checks for consumers

---

## 🚀 Next Steps

1. **Start with RoomManagement Consumers**
   - Copy the consumer examples above
   - Add them to your Infrastructure project
   - Register them in DependencyInjection.cs

2. **Create Message Contracts**
   - Consider creating a shared contracts library
   - Define all message types consistently

3. **Implement Inventory Service**
   - Use the consumer examples as a guide
   - Focus on room availability and hold logic

4. **Add Integration Tests**
   - Test the complete booking flow
   - Verify message delivery and processing 