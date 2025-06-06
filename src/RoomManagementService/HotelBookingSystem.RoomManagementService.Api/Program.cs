using HotelBookingSystem.RoomManagementService.Api.Extensions;
using HotelBookingSystem.RoomManagementService.Application.Queries.GetRoomTypes;
using HotelBookingSystem.RoomManagementService.Infrastructure;
using HotelBookingSystem.RoomManagementService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (.NET Aspire)
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Goal Service API",
        Description = "An ASP.NET Core Web API for managing Goals and their Progress"
    });
});

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetRoomTypesQuery).Assembly));

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapRoomManagementEndpoints();

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<RoomManagementDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
