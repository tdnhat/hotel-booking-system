# 🚀 Messaging Architecture & Consumer Flows

## 📋 Overview

This document outlines the messaging architecture for the Hotel Booking System using MassTransit with RabbitMQ. It provides examples of message publishers, consumers, and complete booking flows between services.

## 🏗️ Service Architecture

- **🏨 RoomManagement Service**: Manages hotels and room types
- **📦 Inventory Service**: Handles room availability and pricing
- **💳 Payment Service**: Processes payments and transactions  
- **📝 Booking Service**: Orchestrates booking workflow (✅ **Implemented**)

## 🔄 Message Flow Patterns

### 📤 **Publishers** (Services that send messages)
- **Events**: Domain events that happened (past tense)
- **Commands**: Requests for other services to do something
- **Queries**: Requests for information

### 📥 **Consumers** (Services that receive messages)
- **Event Handlers**: React to domain events
- **Command Handlers**: Execute requested actions
- **Query Handlers**: Provide requested information

---

## 📨 Message Definitions

### 🏨 **RoomManagement Service Messages**

#### 📤 **Published Events**

```csharp
// Published when a new room type is created
public class RoomTypeCreatedEvent
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; }
    public int MaxOccupancy { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Published when room type pricing changes
public class RoomTypePriceUpdatedEvent
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public decimal NewPrice { get; set; }
    public decimal OldPrice { get; set; }
    public string Currency { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string Reason { get; set; } // "DynamicPricing", "ManualUpdate", etc.
}

// Published when hotel status changes
public class HotelStatusChangedEvent
{
    public Guid HotelId { get; set; }
    public string HotelName { get; set; }
    public string OldStatus { get; set; }
    public string NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
}

// Published when room type is deactivated/discontinued
public class RoomTypeDeactivatedEvent
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; }
    public DateTime DeactivatedAt { get; set; }
    public string Reason { get; set; }
}
```

#### 📥 **Consumed Events**

```csharp
// From Booking Service - to update statistics
public class BookingConfirmedEvent
{
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime BookedAt { get; set; }
}

// From Inventory Service - for dynamic pricing
public class RoomAvailabilityUpdatedEvent
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public decimal CurrentPrice { get; set; }
    public int TotalRooms { get; set; }
}
```

### 📦 **Inventory Service Messages**

#### 📤 **Published Events**

```csharp
// Published when room availability changes
public class RoomAvailabilityUpdatedEvent
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalRooms { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Published when rooms are successfully held
public class RoomHeldEvent
{
    public Guid BookingId { get; set; }
    public Guid HoldId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime HeldAt { get; set; }
}

// Published when room hold fails
public class RoomHoldFailedEvent
{
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RequestedRoomCount { get; set; }
    public int AvailableRoomCount { get; set; }
    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }
}

// Published when rooms are released
public class RoomReleasedEvent
{
    public Guid BookingId { get; set; }
    public Guid HoldId { get; set; }
    public int ReleasedRoomCount { get; set; }
    public string Reason { get; set; } // "Expired", "Cancelled", "PaymentFailed"
    public DateTime ReleasedAt { get; set; }
}
```

#### 📥 **Consumed Commands**

```csharp
// From Booking Service - to hold rooms
public class HoldRoomCommand
{
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCount { get; set; }
    public TimeSpan HoldDuration { get; set; } = TimeSpan.FromMinutes(15);
    public DateTime RequestedAt { get; set; }
}

// From Booking Service - to release held rooms
public class ReleaseRoomHoldCommand
{
    public Guid BookingId { get; set; }
    public Guid HoldId { get; set; }
    public string Reason { get; set; }
    public DateTime RequestedAt { get; set; }
}

// From Booking Service - to confirm booking
public class ConfirmRoomBookingCommand
{
    public Guid BookingId { get; set; }
    public Guid HoldId { get; set; }
    public Guid PaymentId { get; set; }
    public DateTime ConfirmedAt { get; set; }
}
```

#### 📥 **Consumed Events**

```csharp
// From RoomManagement Service
public class RoomTypeCreatedEvent
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; }
    public int MaxOccupancy { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedAt { get; set; }
}

// From RoomManagement Service
public class RoomTypeDeactivatedEvent
{
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime DeactivatedAt { get; set; }
}
```

### 💳 **Payment Service Messages**

#### 📤 **Published Events**

```csharp
// Published when payment processing completes
public class PaymentProcessedEvent
{
    public Guid BookingId { get; set; }
    public Guid PaymentId { get; set; }
    public bool Success { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public string FailureReason { get; set; }
    public DateTime ProcessedAt { get; set; }
}

// Published when refund is processed
public class RefundProcessedEvent
{
    public Guid BookingId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid RefundId { get; set; }
    public decimal RefundAmount { get; set; }
    public string Currency { get; set; }
    public bool Success { get; set; }
    public string RefundTransactionId { get; set; }
    public string FailureReason { get; set; }
    public DateTime ProcessedAt { get; set; }
}
```

#### 📥 **Consumed Commands**

```csharp
// From Booking Service
public class ProcessPaymentCommand
{
    public Guid BookingId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentMethod { get; set; }
    public string CustomerEmail { get; set; }
    public Dictionary<string, string> PaymentDetails { get; set; }
    public DateTime RequestedAt { get; set; }
}

// From Booking Service
public class ProcessRefundCommand
{
    public Guid BookingId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid RefundId { get; set; }
    public decimal RefundAmount { get; set; }
    public string Currency { get; set; }
    public string Reason { get; set; }
    public DateTime RequestedAt { get; set; }
}
```

### 📝 **Booking Service Messages** (✅ **Implemented**)

#### 📤 **Published Events**

```csharp
// Published when booking is confirmed
public class BookingConfirmedEvent
{
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCount { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid PaymentId { get; set; }
    public string TransactionId { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime BookedAt { get; set; }
}

// Published when booking is cancelled
public class BookingCancelledEvent
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; }
    public bool RefundIssued { get; set; }
    public decimal RefundAmount { get; set; }
    public DateTime CancelledAt { get; set; }
}
```

#### 📤 **Published Commands**

```csharp
// To Inventory Service
public class HoldRoomCommand { /* ... */ }
public class ReleaseRoomHoldCommand { /* ... */ }
public class ConfirmRoomBookingCommand { /* ... */ }

// To Payment Service
public class ProcessPaymentCommand { /* ... */ }
public class ProcessRefundCommand { /* ... */ }
```

---

## 🔧 Consumer Implementations

### 🏨 **RoomManagement Service Consumers**

```csharp
// 📥 Handles booking confirmations for statistics
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

// 📥 Handles availability updates for dynamic pricing
public class RoomAvailabilityUpdatedConsumer : IConsumer<RoomAvailabilityUpdatedEvent>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RoomAvailabilityUpdatedConsumer> _logger;

    public async Task Consume(ConsumeContext<RoomAvailabilityUpdatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing availability update for RoomType: {RoomTypeId}", message.RoomTypeId);

        try
        {
            var hotel = await _hotelRepository.GetByIdWithRoomTypesAsync(HotelId.From(message.HotelId));
            if (hotel == null) return;

            var roomType = hotel.GetRoomType(RoomTypeId.From(message.RoomTypeId));
            
            // Dynamic pricing based on availability
            var occupancyRate = (double)(message.TotalRooms - message.AvailableRooms) / message.TotalRooms;
            
            if (occupancyRate > 0.8) // High demand - increase price
            {
                var newPrice = Money.Create(message.CurrentPrice * 1.2m, "USD");
                roomType.UpdatePrice(newPrice);
                
                await _publishEndpoint.Publish(new RoomTypePriceUpdatedEvent
                {
                    HotelId = message.HotelId,
                    RoomTypeId = message.RoomTypeId,
                    NewPrice = newPrice.Amount,
                    OldPrice = message.CurrentPrice,
                    Currency = newPrice.Currency,
                    EffectiveDate = message.Date,
                    Reason = "DynamicPricing - High Demand"
                });
            }
            else if (occupancyRate < 0.3) // Low demand - decrease price
            {
                var newPrice = Money.Create(message.CurrentPrice * 0.9m, "USD");
                roomType.UpdatePrice(newPrice);
                
                await _publishEndpoint.Publish(new RoomTypePriceUpdatedEvent
                {
                    HotelId = message.HotelId,
                    RoomTypeId = message.RoomTypeId,
                    NewPrice = newPrice.Amount,
                    OldPrice = message.CurrentPrice,
                    Currency = newPrice.Currency,
                    EffectiveDate = message.Date,
                    Reason = "DynamicPricing - Low Demand"
                });
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

### 📦 **Inventory Service Consumers**

```csharp
// 📥 Initializes inventory when new room types are created
public class RoomTypeCreatedConsumer : IConsumer<RoomTypeCreatedEvent>
{
    private readonly IRoomInventoryService _inventoryService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RoomTypeCreatedConsumer> _logger;

    public async Task Consume(ConsumeContext<RoomTypeCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Creating inventory for new RoomType: {RoomTypeId}", message.RoomTypeId);

        try
        {
            // Initialize inventory for the next 365 days
            await _inventoryService.InitializeRoomTypeInventoryAsync(
                message.HotelId,
                message.RoomTypeId,
                message.BasePrice,
                defaultRoomCount: 10); // Default room count
                
            // Publish availability event
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

// 📥 Handles room hold requests
public class HoldRoomConsumer : IConsumer<HoldRoomCommand>
{
    private readonly IRoomInventoryService _inventoryService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<HoldRoomConsumer> _logger;

    public async Task Consume(ConsumeContext<HoldRoomCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Processing room hold request for BookingId: {BookingId}", command.BookingId);

        try
        {
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
            
            _logger.LogInformation("Successfully held {RoomCount} rooms for BookingId: {BookingId}", 
                command.RoomCount, command.BookingId);
        }
        catch (InsufficientInventoryException ex)
        {
            await _publishEndpoint.Publish(new RoomHoldFailedEvent
            {
                BookingId = command.BookingId,
                HotelId = command.HotelId,
                RoomTypeId = command.RoomTypeId,
                CheckIn = command.CheckIn,
                CheckOut = command.CheckOut,
                RequestedRoomCount = command.RoomCount,
                AvailableRoomCount = ex.AvailableRooms,
                Reason = ex.Message,
                FailedAt = DateTime.UtcNow
            });
            
            _logger.LogWarning("Room hold failed for BookingId: {BookingId} - {Reason}", 
                command.BookingId, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing room hold for BookingId: {BookingId}", command.BookingId);
            throw;
        }
    }
}

// 📥 Handles room hold releases
public class ReleaseRoomHoldConsumer : IConsumer<ReleaseRoomHoldCommand>
{
    private readonly IRoomInventoryService _inventoryService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ReleaseRoomHoldConsumer> _logger;

    public async Task Consume(ConsumeContext<ReleaseRoomHoldCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Releasing room hold for BookingId: {BookingId}", command.BookingId);

        try
        {
            var releasedCount = await _inventoryService.ReleaseHoldAsync(command.HoldId, command.Reason);
            
            await _publishEndpoint.Publish(new RoomReleasedEvent
            {
                BookingId = command.BookingId,
                HoldId = command.HoldId,
                ReleasedRoomCount = releasedCount,
                Reason = command.Reason,
                ReleasedAt = DateTime.UtcNow
            });
            
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

### 💳 **Payment Service Consumers**

```csharp
// 📥 Processes payment requests
public class ProcessPaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    private readonly IPaymentService _paymentService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ProcessPaymentConsumer> _logger;

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Processing payment for BookingId: {BookingId}, Amount: {Amount}", 
            command.BookingId, command.Amount);

        try
        {
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
        catch (PaymentFailedException ex)
        {
            await _publishEndpoint.Publish(new PaymentProcessedEvent
            {
                BookingId = command.BookingId,
                PaymentId = command.PaymentId,
                Success = false,
                Amount = command.Amount,
                Currency = command.Currency,
                PaymentMethod = command.PaymentMethod,
                FailureReason = ex.Message,
                ProcessedAt = DateTime.UtcNow
            });
            
            _logger.LogWarning("Payment failed for BookingId: {BookingId} - {Reason}", 
                command.BookingId, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for BookingId: {BookingId}", command.BookingId);
            throw;
        }
    }
}

// 📥 Processes refund requests
public class ProcessRefundConsumer : IConsumer<ProcessRefundCommand>
{
    private readonly IPaymentService _paymentService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ProcessRefundConsumer> _logger;

    public async Task Consume(ConsumeContext<ProcessRefundCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Processing refund for BookingId: {BookingId}, Amount: {Amount}", 
            command.BookingId, command.RefundAmount);

        try
        {
            var result = await _paymentService.ProcessRefundAsync(
                command.PaymentId,
                command.RefundId,
                command.RefundAmount,
                command.Currency,
                command.Reason);
                
            await _publishEndpoint.Publish(new RefundProcessedEvent
            {
                BookingId = command.BookingId,
                PaymentId = command.PaymentId,
                RefundId = command.RefundId,
                RefundAmount = command.RefundAmount,
                Currency = command.Currency,
                Success = true,
                RefundTransactionId = result.RefundTransactionId,
                ProcessedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Refund successful for BookingId: {BookingId}, RefundTransactionId: {RefundTransactionId}", 
                command.BookingId, result.RefundTransactionId);
        }
        catch (RefundFailedException ex)
        {
            await _publishEndpoint.Publish(new RefundProcessedEvent
            {
                BookingId = command.BookingId,
                PaymentId = command.PaymentId,
                RefundId = command.RefundId,
                RefundAmount = command.RefundAmount,
                Currency = command.Currency,
                Success = false,
                FailureReason = ex.Message,
                ProcessedAt = DateTime.UtcNow
            });
            
            _logger.LogWarning("Refund failed for BookingId: {BookingId} - {Reason}", 
                command.BookingId, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for BookingId: {BookingId}", command.BookingId);
            throw;
        }
    }
}
```

---

## ⚙️ MassTransit Configuration

### 🏨 **RoomManagement Service Configuration**

```csharp
// In RoomManagement.Infrastructure/DependencyInjection.cs
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

        // Configure specific endpoints
        cfg.ReceiveEndpoint("room-management-booking-confirmed", e =>
        {
            e.ConfigureConsumer<BookingConfirmedConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("room-management-availability-updated", e =>
        {
            e.ConfigureConsumer<RoomAvailabilityUpdatedConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

### 📦 **Inventory Service Configuration**

```csharp
// In Inventory.Infrastructure/DependencyInjection.cs
services.AddMassTransit(config =>
{
    // Register consumers
    config.AddConsumer<RoomTypeCreatedConsumer>();
    config.AddConsumer<HoldRoomConsumer>();
    config.AddConsumer<ReleaseRoomHoldConsumer>();
    config.AddConsumer<ConfirmRoomBookingConsumer>();
    
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(connectionString);
        
        // Configure command endpoints
        cfg.ReceiveEndpoint("inventory-hold-room", e =>
        {
            e.ConfigureConsumer<HoldRoomConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("inventory-release-hold", e =>
        {
            e.ConfigureConsumer<ReleaseRoomHoldConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

### 💳 **Payment Service Configuration**

```csharp
// In Payment.Infrastructure/DependencyInjection.cs
services.AddMassTransit(config =>
{
    // Register consumers
    config.AddConsumer<ProcessPaymentConsumer>();
    config.AddConsumer<ProcessRefundConsumer>();
    
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(connectionString);
        
        // Configure command endpoints
        cfg.ReceiveEndpoint("payment-process", e =>
        {
            e.ConfigureConsumer<ProcessPaymentConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("payment-refund", e =>
        {
            e.ConfigureConsumer<ProcessRefundConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

---

## 🔄 Complete Booking Flow Example

### **Happy Path Flow**

```
1. 🚀 User initiates booking request
   └── Booking Service receives HTTP request

2. 📤 Booking → Inventory: HoldRoomCommand
   └── Request to hold specific rooms

3. 📥 Inventory processes hold request
   └── Checks availability and creates hold

4. 📤 Inventory → Booking: RoomHeldEvent
   └── Confirms rooms are held with hold details

5. 📤 Booking → Payment: ProcessPaymentCommand
   └── Initiates payment processing

6. 📥 Payment processes payment
   └── Charges customer's payment method

7. 📤 Payment → Booking: PaymentProcessedEvent (Success)
   └── Confirms payment was successful

8. 📤 Booking → Inventory: ConfirmRoomBookingCommand
   └── Confirms the booking and converts hold to booking

9. 📤 Booking → All Services: BookingConfirmedEvent
   └── Notifies all services of successful booking

10. 📤 Inventory → All Services: RoomAvailabilityUpdatedEvent
    └── Updates availability after booking confirmation
```

### **Failure Scenarios**

#### **Payment Failure**
```
1-6. Same as happy path through payment processing

7. 📤 Payment → Booking: PaymentProcessedEvent (Failed)
   └── Payment failed due to insufficient funds

8. 📤 Booking → Inventory: ReleaseRoomHoldCommand
   └── Release held rooms due to payment failure

9. 📤 Inventory → All Services: RoomReleasedEvent
   └── Notifies that rooms have been released

10. 📤 Booking → User: BookingFailedEvent
    └── Notifies user of booking failure
```

#### **Insufficient Inventory**
```
1-2. Same as happy path through hold request

3. 📥 Inventory processes hold request
   └── Insufficient rooms available

4. 📤 Inventory → Booking: RoomHoldFailedEvent
   └── Notifies that hold request failed

5. 📤 Booking → User: BookingFailedEvent
   └── Notifies user that booking failed due to availability
```

---

## 🛠️ Implementation Status

- ✅ **Booking Service**: Fully implemented with saga pattern
- ⏳ **RoomManagement Service**: Infrastructure completed, consumers pending
- ⏳ **Inventory Service**: Not yet implemented
- ⏳ **Payment Service**: Not yet implemented

## 📝 Next Steps

1. **Implement Inventory Service**
   - Create inventory domain models
   - Implement room availability tracking
   - Add hold/release functionality
   - Create consumers for room type events

2. **Implement Payment Service**
   - Create payment processing infrastructure
   - Implement payment gateway integration
   - Add refund functionality
   - Create consumers for payment commands

3. **Add RoomManagement Service Consumers**
   - Implement BookingConfirmedConsumer
   - Implement RoomAvailabilityUpdatedConsumer
   - Add dynamic pricing logic

4. **Testing & Integration**
   - Create integration tests for message flows
   - Test failure scenarios and retries
   - Validate end-to-end booking flows

---

## 🔗 Related Documentation

- [Design Plan](design-plan.md)
- [Use Cases](use-cases.md)
- [Features](features.md)
- [Project Scope](scope.md) 