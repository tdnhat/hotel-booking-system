var builder = DistributedApplication.CreateBuilder(args);

var bookingDb = builder.AddPostgres("postgres-booking")
                        .WithDataVolume() // Persis Data
                        .AddDatabase("bookingdb"); // Must match booking service's db 

var bookingService = builder.AddProject<Projects.HotelBookingSystem_BookingService_Api>("booking-service")
                            .WithReference(bookingDb);

builder.Build().Run();
