# Multi-stage build for BookingService API
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Install Aspire workload
RUN dotnet workload install aspire

# Copy project files
COPY ["src/BookingService/HotelBookingSystem.BookingService.Api/HotelBookingSystem.BookingService.Api.csproj", "BookingService/HotelBookingSystem.BookingService.Api/"]
COPY ["src/BookingService/HotelBookingSystem.BookingService.Application/HotelBookingSystem.BookingService.Application.csproj", "BookingService/HotelBookingSystem.BookingService.Application/"]
COPY ["src/BookingService/HotelBookingSystem.BookingService.Domain/HotelBookingSystem.BookingService.Domain.csproj", "BookingService/HotelBookingSystem.BookingService.Domain/"]
COPY ["src/BookingService/HotelBookingSystem.BookingService.Infrastructure/HotelBookingSystem.BookingService.Infrastructure.csproj", "BookingService/HotelBookingSystem.BookingService.Infrastructure/"]
COPY ["src/HotelBookingSystem.ServiceDefaults/HotelBookingSystem.ServiceDefaults.csproj", "HotelBookingSystem.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "BookingService/HotelBookingSystem.BookingService.Api/HotelBookingSystem.BookingService.Api.csproj"

# Copy source code
COPY src/ .

# Build application
RUN dotnet build "BookingService/HotelBookingSystem.BookingService.Api/HotelBookingSystem.BookingService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookingService/HotelBookingSystem.BookingService.Api/HotelBookingSystem.BookingService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelBookingSystem.BookingService.Api.dll"]