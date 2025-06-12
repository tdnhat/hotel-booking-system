var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres")
    .WithImageTag("16.3-alpine");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithImage("rabbitmq")
    .WithImageTag("3.13.3-management-alpine");

var bookingDb = postgres.AddDatabase("bookingdb");
var inventoryDb = postgres.AddDatabase("inventorydb");

var bookingService = builder.AddProject<Projects.HotelBookingSystem_BookingService_Api>("bookingservice")
    .WithReference(bookingDb)
    .WithReference(rabbitmq);

var inventoryService = builder.AddProject<Projects.HotelBookingSystem_InventoryService_Api>("inventoryservice")
    .WithReference(inventoryDb)
    .WithReference(rabbitmq);

builder.Build().Run();
