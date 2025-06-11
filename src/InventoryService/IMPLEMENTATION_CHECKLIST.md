# InventoryService Implementation Checklist

## 📋 Implementation Progress Tracker

**Based on**: [Technical Design Document](./technical-design-inventory-service.md)  
**Started**: 2024-01-15  
**Target Completion**: TBD  

---

## 🏗️ **Phase 1: Foundation & Domain Layer**

### **Domain Entities & Models**
- [x] Create `src/HotelBookingSystem.InventoryService.Domain/Entities/` folder (Completed)
- [x] Implement `RoomInventory.cs` entity (Completed)
  - [x] Properties: Id, HotelId, RoomTypeId, RoomNumber, TotalRooms (Completed)
  - [x] Navigation properties to RoomHold and AvailabilityCalendar (Completed)
  - [x] Business methods: CheckAvailability(), GetAvailableRooms() (Completed)
- [x] Implement `RoomHold.cs` entity (Completed)
  - [x] Properties: Id, BookingId, RoomInventoryId, HoldReference, HeldAt, HeldUntil, Status (Completed)
  - [x] Business methods: IsExpired(), Release(), Extend() (Completed)
  - [x] Domain validation rules (Completed)
- [x] Implement `AvailabilityCalendar.cs` entity (Completed)
  - [x] Properties: Id, RoomInventoryId, AvailabilityDate, AvailableRooms, HeldRooms, BookedRooms (Completed)
  - [x] Business methods: UpdateAvailability(), CalculateAvailable() (Completed)
- [x] Create `src/HotelBookingSystem.InventoryService.Domain/Enums/` folder (Completed)
- [x] Implement `RoomHoldStatus.cs` enum (Active, Expired, Released, Confirmed) (Completed)
- [x] Create `src/HotelBookingSystem.InventoryService.Domain/ValueObjects/` folder (Completed)
- [x] Implement `HoldReference.cs` value object (Completed)
- [x] Implement `DateRange.cs` value object (Completed)

### **Domain Repositories & Interfaces**
- [x] Create `src/HotelBookingSystem.InventoryService.Domain/Repositories/` folder (Completed)
- [x] Define `IRoomInventoryRepository.cs` interface (Completed)
- [x] Define `IRoomHoldRepository.cs` interface (Completed)
- [x] Define `IAvailabilityCalendarRepository.cs` interface (Completed)

---

## 🎯 **Phase 2: Application Layer**

### **Commands & Command Handlers**
- [ ] Create `src/HotelBookingSystem.InventoryService.Application/Commands/` folder
- [ ] Create `CreateRoomHold/` subfolder
  - [ ] Implement `CreateRoomHoldCommand.cs`
  - [ ] Implement `CreateRoomHoldCommandHandler.cs`
  - [ ] Add FluentValidation validator
- [ ] Create `ReleaseRoomHold/` subfolder
  - [ ] Implement `ReleaseRoomHoldCommand.cs`
  - [ ] Implement `ReleaseRoomHoldCommandHandler.cs`
  - [ ] Add FluentValidation validator

### **Queries & Query Handlers**
- [ ] Create `src/HotelBookingSystem.InventoryService.Application/Queries/` folder
- [ ] Create `CheckAvailability/` subfolder
  - [ ] Implement `CheckAvailabilityQuery.cs`
  - [ ] Implement `CheckAvailabilityQueryHandler.cs`
- [ ] Create `GetRoomHoldStatus/` subfolder
  - [ ] Implement `GetRoomHoldStatusQuery.cs`
  - [ ] Implement `GetRoomHoldStatusQueryHandler.cs`

### **DTOs & Response Models**
- [ ] Create `src/HotelBookingSystem.InventoryService.Application/DTOs/` folder
- [ ] Implement `AvailabilityCheckRequest.cs`
- [ ] Implement `AvailabilityCheckResponse.cs`
- [ ] Implement `RoomHoldRequest.cs`
- [ ] Implement `RoomHoldResponse.cs`
- [ ] Implement `RoomHoldStatusResponse.cs`

### **Application Interfaces**
- [ ] Create `src/HotelBookingSystem.InventoryService.Application/Interfaces/` folder
- [ ] Define `IRoomManagementClient.cs` interface
- [ ] Define `IInventoryDbContext.cs` interface
- [ ] Define `IDateTimeProvider.cs` interface

### **Application Services**
- [ ] Create `src/HotelBookingSystem.InventoryService.Application/Services/` folder
- [ ] Implement `InventoryService.cs` (core business logic)
- [ ] Implement `HoldTimeoutService.cs` (background service for expired holds)

### **Dependency Injection**
- [ ] Create `DependencyInjection.cs` in Application project
- [ ] Register MediatR and handlers
- [ ] Register FluentValidation
- [ ] Register application services

---

## 🔧 **Phase 3: Infrastructure Layer**

### **Database Context & Configuration**
- [ ] Create `src/HotelBookingSystem.InventoryService.Infrastructure/Data/` folder
- [ ] Implement `InventoryDbContext.cs`
  - [ ] DbSet properties for all entities
  - [ ] OnModelCreating configuration
  - [ ] Add MassTransit outbox configuration
- [ ] Create `Configurations/` subfolder
  - [ ] Implement `RoomInventoryConfiguration.cs`
  - [ ] Implement `RoomHoldConfiguration.cs`
  - [ ] Implement `AvailabilityCalendarConfiguration.cs`

### **Repository Implementations**
- [ ] Create `src/HotelBookingSystem.InventoryService.Infrastructure/Repositories/` folder
- [ ] Implement `RoomInventoryRepository.cs`
- [ ] Implement `RoomHoldRepository.cs`
- [ ] Implement `AvailabilityCalendarRepository.cs`

### **External Service Clients**
- [ ] Create `src/HotelBookingSystem.InventoryService.Infrastructure/Clients/` folder
- [ ] Implement `RoomManagementClient.cs`
  - [ ] HTTP client for RoomManagementService
  - [ ] Methods: GetRoomTypeAsync(), GetRoomsByTypeAsync()
  - [ ] Error handling and retry policies

### **Message Consumers**
- [ ] Create `src/HotelBookingSystem.InventoryService.Infrastructure/Consumers/` folder
- [ ] Implement `HoldRoomConsumer.cs`
  - [ ] Handle HoldRoom command from BookingService
  - [ ] Publish RoomHeld or RoomHoldFailed events
- [ ] Implement `ReleaseRoomConsumer.cs`
  - [ ] Handle ReleaseRoom command from BookingService
  - [ ] Publish RoomReleased event

### **Background Services**
- [ ] Create `src/HotelBookingSystem.InventoryService.Infrastructure/BackgroundServices/` folder
- [ ] Implement `HoldExpirationService.cs`
  - [ ] Periodic check for expired holds
  - [ ] Automatic cleanup and event publishing

### **Dependency Injection**
- [ ] Create `DependencyInjection.cs` in Infrastructure project
- [ ] Register Entity Framework DbContext
- [ ] Register repositories
- [ ] Register MassTransit with consumers
- [ ] Register HTTP clients
- [ ] Register background services

---

## 🌐 **Phase 4: API Layer**

### **Minimal API Endpoints**
- [ ] Create `src/HotelBookingSystem.InventoryService.Api/Endpoints/` folder
- [ ] Implement `InventoryEndpoints.cs`
  - [ ] Create `MapInventoryEndpoints()` extension method
  - [ ] GET `/api/v1/inventory/availability` endpoint with query parameters
  - [ ] POST `/api/v1/inventory/holds` endpoint for creating room holds
  - [ ] DELETE `/api/v1/inventory/holds/{holdReference}` endpoint for releasing holds
  - [ ] GET `/api/v1/inventory/holds/{holdReference}` endpoint for hold status
  - [ ] Add proper OpenAPI documentation with `.WithSummary()` and `.WithDescription()`
  - [ ] Configure response types with `.Produces<T>()`

### **Endpoint Extensions & Versioning**
- [ ] Create `src/HotelBookingSystem.InventoryService.Api/Extensions/` folder
- [ ] Implement `EndpointExtensions.cs`
  - [ ] Create `MapInventoryEndpoints()` method to register all endpoints
  - [ ] Setup API versioning with `/api/v1` group
  - [ ] Add endpoint tags for Swagger organization
  - [ ] Prepare for future v2 endpoints structure

### **Program.cs Configuration**
- [ ] Update `src/HotelBookingSystem.InventoryService.Api/Program.cs`
  - [ ] Add service registrations (Application + Infrastructure)
  - [ ] Configure Serilog logging
  - [ ] Add .NET Aspire ServiceDefaults
  - [ ] Configure Swagger/OpenAPI documentation
  - [ ] Setup middleware pipeline
  - [ ] Add health checks configuration
  - [ ] Call `app.MapInventoryEndpoints()` for endpoint registration
  - [ ] Add database auto-migration for development environment

### **API Models & Validation**
- [ ] Create request/response models within endpoint methods or as records
- [ ] Add data annotations for validation where needed
- [ ] Implement proper error handling with `Results.BadRequest()`, `Results.NotFound()`, etc.
- [ ] Use `IResult` return type for all endpoint methods

### **Middleware & Configuration**
- [ ] Add global exception handling middleware
- [ ] Configure CORS policy
- [ ] Add request/response logging
- [ ] Configure authentication and authorization
- [ ] Setup health checks for dependencies

---

## 💾 **Phase 5: Database & Migrations**

### **Database Setup**
- [ ] Install required NuGet packages in Infrastructure project:
  - [ ] `Microsoft.EntityFrameworkCore`
  - [ ] `Npgsql.EntityFrameworkCore.PostgreSQL`
  - [ ] `Microsoft.EntityFrameworkCore.Design`
- [ ] Configure connection strings in `appsettings.json`
- [ ] Create initial EF Core migration
  - [ ] Run: `dotnet ef migrations add InitialCreate`
- [ ] Create database schema
  - [ ] Run: `dotnet ef database update`

### **Seed Data**
- [ ] Create `src/HotelBookingSystem.InventoryService.Infrastructure/Data/Seed/` folder
- [ ] Implement `InventoryDataSeeder.cs`
- [ ] Add sample room inventory data for testing
- [ ] Configure seed data in Program.cs

---

## 📨 **Phase 6: Message Contracts & Integration**

### **Shared Message Contracts**
- [ ] Create `src/HotelBookingSystem.InventoryService.Domain/Messages/` folder
- [ ] Create `Commands/` subfolder
  - [ ] Implement `HoldRoom.cs` message contract
  - [ ] Implement `ReleaseRoom.cs` message contract
- [ ] Create `Events/` subfolder
  - [ ] Implement `RoomHeld.cs` event contract
  - [ ] Implement `RoomHoldFailed.cs` event contract
  - [ ] Implement `RoomReleased.cs` event contract

### **Update BookingService Integration**
- [ ] Update BookingService consumers to call InventoryService:
  - [ ] Update `HoldRoomConsumer.cs` to use HTTP client
  - [ ] Update `ReleaseRoomConsumer.cs` to use HTTP client
  - [ ] Add `IInventoryServiceClient` interface
  - [ ] Implement HTTP client for InventoryService

---

## 🧪 **Phase 7: Testing**

### **Unit Tests**
- [ ] Create `tests/HotelBookingSystem.InventoryService.Domain.Tests/` project
- [ ] Test domain entities and business logic
- [ ] Test value objects validation
- [ ] Test domain repository interfaces

- [ ] Create `tests/HotelBookingSystem.InventoryService.Application.Tests/` project
- [ ] Test command handlers
- [ ] Test query handlers
- [ ] Test application services
- [ ] Mock external dependencies

### **Integration Tests**
- [ ] Create `tests/HotelBookingSystem.InventoryService.Integration.Tests/` project
- [ ] Test database operations with TestContainers
- [ ] Test MassTransit message publishing/consuming
- [ ] Test HTTP client integration with RoomManagementService
- [ ] Test API endpoints end-to-end

### **Performance Tests**
- [ ] Create load tests for availability checking (1000 concurrent requests)
- [ ] Test room hold operation throughput
- [ ] Monitor database performance under load
- [ ] Memory usage and garbage collection analysis

---

## 🚀 **Phase 8: Deployment & Configuration**

### **.NET Aspire Integration**
- [ ] Update `AppHost/Program.cs`:
  - [ ] Add InventoryService database configuration
  - [ ] Add InventoryService project reference
  - [ ] Configure service dependencies
- [ ] Update `ServiceDefaults` for logging and monitoring
- [ ] Configure health checks for InventoryService

### **Environment Configuration**
- [ ] Configure development environment settings
- [ ] Configure staging environment settings
- [ ] Configure production environment settings
- [ ] Set up environment-specific connection strings

### **Docker Configuration**
- [ ] Create `Dockerfile` for InventoryService
- [ ] Add to `docker-compose.yml` if used
- [ ] Configure container networking

---

## 📊 **Phase 9: Monitoring & Observability**

### **Logging & Metrics**
- [ ] Add structured logging with Serilog
- [ ] Implement custom metrics collection
- [ ] Add performance counters
- [ ] Configure distributed tracing

### **Health Checks**
- [ ] Add database connectivity health check
- [ ] Add RabbitMQ connectivity health check
- [ ] Add external service health checks
- [ ] Configure health check endpoints

---

## ✅ **Phase 10: Documentation & Finalization**

### **API Documentation**
- [ ] Generate OpenAPI/Swagger documentation
- [ ] Add XML documentation comments
- [ ] Create API usage examples
- [ ] Document error codes and responses

### **Architecture Documentation**
- [ ] Update system architecture diagrams
- [ ] Document service integration patterns
- [ ] Create deployment guides
- [ ] Update README files

### **Final Review**
- [ ] Code review and refactoring
- [ ] Security review
- [ ] Performance optimization
- [ ] Final testing and validation

---

## 🎯 **Success Criteria**

- [ ] All API endpoints respond within performance requirements (200ms availability, 500ms holds)
- [ ] No double-booking scenarios possible
- [ ] Hold timeouts work reliably (99.9% accuracy)
- [ ] BookingService saga integration works end-to-end
- [ ] All tests pass (unit, integration, performance)
- [ ] Service can handle target load (1000 concurrent availability checks, 200 hold operations)
- [ ] Monitoring and alerting fully configured
- [ ] Documentation complete and up-to-date

---

**Progress Tracking:**
- **Total Tasks**: 120+
- **Completed**: 17 (Phase 1 Complete)
- **In Progress**: 0
- **Remaining**: 103+

**Estimated Timeline**: 6-8 weeks for full implementation
**Priority**: High - Critical for BookingService saga functionality 