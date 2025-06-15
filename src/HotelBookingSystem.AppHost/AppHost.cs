var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithImage("rabbitmq");

var bookingDb = postgres.AddDatabase("bookingdb");
var inventoryDb = postgres.AddDatabase("inventorydb");
var roomManagementDb = postgres.AddDatabase("roommanagementdb");

var bookingService = builder.AddProject<Projects.HotelBookingSystem_BookingService_Api>("bookingservice")
    .WithReference(bookingDb)
    .WithReference(rabbitmq);

var inventoryService = builder.AddProject<Projects.HotelBookingSystem_InventoryService_Api>("inventoryservice")
    .WithReference(inventoryDb)
    .WithReference(rabbitmq);

var roomManagementService = builder.AddProject<Projects.HotelBookingSystem_RoomManagementService_Api>("roommanagementservice")
    .WithReference(roomManagementDb)
    .WithReference(rabbitmq);

builder.Build().Run();
