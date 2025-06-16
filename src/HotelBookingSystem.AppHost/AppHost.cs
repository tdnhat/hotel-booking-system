var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure services with persistence for development
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithImage("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent);

// Databases
var bookingDb = postgres.AddDatabase("bookingdb");
var inventoryDb = postgres.AddDatabase("inventorydb");
var roomManagementDb = postgres.AddDatabase("roommanagementdb");

// Core services - Room Management should start first as other services depend on it
var roomManagementService = builder.AddProject<Projects.HotelBookingSystem_RoomManagementService_Api>("roommanagementservice")
    .WithReference(roomManagementDb)
    .WaitFor(roomManagementDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithHttpHealthCheck("/health");

// Inventory Service - depends on Room Management data for room types and hotels
var inventoryService = builder.AddProject<Projects.HotelBookingSystem_InventoryService_Api>("inventoryservice")
    .WithReference(inventoryDb)
    .WaitFor(inventoryDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(roomManagementService) // Wait for room management to be ready
    .WithHttpHealthCheck("/health");

// Booking Service - orchestrates the booking process using both Room Management and Inventory
var bookingService = builder.AddProject<Projects.HotelBookingSystem_BookingService_Api>("bookingservice")
    .WithReference(bookingDb)
    .WaitFor(bookingDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(roomManagementService) // Needs room data
    .WaitFor(inventoryService)      // Needs inventory for availability
    .WithHttpHealthCheck("/health");

builder.Build().Run();
