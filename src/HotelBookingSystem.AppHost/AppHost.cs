var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure components
var postgres = builder.AddPostgres("postgres")
                     .WithDataVolume()
                     .WithPgAdmin();

var redis = builder.AddRedis("redis")
                  .WithDataVolume();

var rabbitmq = builder.AddRabbitMQ("messaging")
                     .WithDataVolume()
                     .WithManagementPlugin();

// Databases  
var bookingDb = postgres.AddDatabase("bookingdb");
var roomManagementDb = postgres.AddDatabase("roommanagementdb");

// API Services
var bookingService = builder.AddProject<Projects.HotelBookingSystem_BookingService_Api>("booking-service")
                            .WithReference(bookingDb)
                            .WithReference(redis)
                            .WithReference(rabbitmq);

var roomManagementService = builder.AddProject<Projects.HotelBookingSystem_RoomManagementService_Api>("room-management-service")
                                   .WithReference(roomManagementDb)
                                   .WithReference(redis)
                                   .WithReference(rabbitmq);

// Enable service-to-service communication
bookingService.WithReference(roomManagementService);
roomManagementService.WithReference(bookingService);

builder.Build().Run();
