var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                     .WithDataVolume();

var bookingDb = postgres.AddDatabase("bookingdb");
var roomManagementDb = postgres.AddDatabase("roommanagementdb");

var bookingService = builder.AddProject<Projects.HotelBookingSystem_BookingService_Api>("booking-service")
                            .WithReference(bookingDb);

var roomManagementService = builder.AddProject<Projects.HotelBookingSystem_RoomManagementService_Api>("room-management-service")
                                   .WithReference(roomManagementDb);

builder.Build().Run();
