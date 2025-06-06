# Overview: BookingService Microservice

This document provides a comprehensive overview of the `BookingService` microservice for the Hotel Booking System, detailing its purpose, technology stack, architecture, key components, and core workflows before implementation begins.

## 1. Purpose

The primary responsibility of the `BookingService` is to manage the entire lifecycle of a guest's booking request. This includes:

*   Receiving booking requests initiated by guests via the frontend.
*   Orchestrating the multi-step booking process involving inventory checks (simulated) and payment processing (simulated).
*   Maintaining the state of each booking request throughout its lifecycle.
*   Providing real-time status updates back to the guest.
*   Handling failures and compensations gracefully to ensure system consistency.
*   Providing API endpoints for initiating bookings and querying booking status.

## 2. Technology Stack

Based on the overall project requirements and the recommended implementation approach, the `BookingService` will utilize the following technologies:

*   **Runtime/Framework:** .NET 8 (or latest LTS)
*   **Web API Framework:** ASP.NET Core
*   **Data Access:** Entity Framework Core (EF Core) with a code-first approach.
*   **Database:** PostgreSQL (recommended) or SQL Server Express.
*   **Messaging & Saga Orchestration:** MassTransit library using the RabbitMQ transport.
*   **Real-time Communication:** ASP.NET Core SignalR (for pushing status updates).
*   **Dependency Injection:** Built-in .NET Dependency Injection container.
*   **Testing:** xUnit/NUnit for unit tests, `TestContainers` and `Microsoft.AspNetCore.Mvc.Testing` for integration tests.
*   **Observability:** Integration with .NET Aspire `ServiceDefaults` for logging, tracing (OpenTelemetry), and health checks.

## 3. Architecture: Clean Architecture

The `BookingService` will strictly adhere to Clean Architecture principles, structured into four distinct projects within the `src/BookingService/` directory:

1.  **`HotelBookingSystem.BookingService.Domain` (Class Library):**
    *   **Purpose:** Contains the core business logic and models, independent of any infrastructure concerns.
    *   **Key Components:**
        *   Entities: `BookingState` (representing the Saga instance and booking details), potentially `RoomTypeInfo` (if needed within the booking context).
        *   Value Objects: e.g., `BookingId`, `RoomTypeId`, `Price`.
        *   Domain Events (Optional): Events raised by domain entities (e.g., `BookingConfirmedDomainEvent`).
    *   **Dependencies:** None (on other layers).

2.  **`HotelBookingSystem.BookingService.Application` (Class Library):**
    *   **Purpose:** Orchestrates the application's use cases and contains application-specific business logic.
    *   **Key Components:**
        *   Commands & Queries (e.g., using MediatR or simple service classes): `InitiateBookingCommand`, `GetBookingStatusQuery`.
        *   Handlers/Services: Logic to process commands/queries.
        *   Interfaces: Abstractions for infrastructure concerns (`IBookingRepository`, `IMessagePublisher`, `IInventoryServiceClient`, `IPaymentServiceClient`, `ISignalRNotifier`).
        *   DTOs (Data Transfer Objects): Used for API communication and potentially messaging.
    *   **Dependencies:** `Domain` layer.

3.  **`HotelBookingSystem.BookingService.Infrastructure` (Class Library):**
    *   **Purpose:** Implements the interfaces defined in the Application layer, handling external concerns like database access, messaging, etc.
    *   **Key Components:**
        *   EF Core: `BookingDbContext`, Repository implementations (`BookingRepository`).
        *   MassTransit: Configuration, Saga definition (`BookingSagaStateMachine`), Consumers (for Saga replies or other events), Message publisher implementation.
        *   SignalR: Hub context implementation (`SignalRNotifier`).
        *   Simulated Clients: Implementations for `IInventoryServiceClient`, `IPaymentServiceClient` (initially simulated).
        *   Migrations: EF Core database migrations.
    *   **Dependencies:** `Application` layer.

4.  **`HotelBookingSystem.BookingService.Api` (ASP.NET Core Web API):**
    *   **Purpose:** Exposes the service's functionality via a RESTful API and acts as the entry point.
    *   **Key Components:**
        *   Controllers: `BookingsController` (`POST /api/bookings`, `GET /api/bookings/{id}/status`).
        *   `Program.cs`: Service registration (DI), middleware configuration, Aspire `AddServiceDefaults()` integration.
        *   API Models/ViewModels: Request/response models specific to the API.
    *   **Dependencies:** `Application`, `Infrastructure` (via Dependency Injection).

## 4. Key Components & Workflows

*   **API Endpoints:**
    *   `POST /api/bookings`: Receives the initial booking request (e.g., containing `RoomTypeId`). Validates the request and publishes a `BookingRequested` event (or similar command) to initiate the Saga.
    *   `GET /api/bookings/{bookingId}/status`: Queries the current state of the booking Saga instance from the database via the Application layer.
*   **Booking Saga (`BookingSagaStateMachine`):**
    *   **Orchestrator:** The core of the booking process, implemented as a MassTransit State Machine Saga.
    *   **State:** Persisted using the `BookingState` entity in the database.
    *   **Workflow:** Defines the sequence of operations, states, and transitions:
        1.  **Initial State:** `Submitted` (upon receiving `BookingRequested`).
        2.  **Activity:** Send `HoldRoom` command (to simulated Inventory Service).
        3.  **Transition:** Move to `RoomHoldRequested` state.
        4.  **Event:** Wait for `RoomHeld` or `RoomHoldFailed` event.
        5.  **If `RoomHeld`:**
            *   **Activity:** Send `ProcessPayment` command (to simulated Payment Service).
            *   **Transition:** Move to `PaymentProcessing` state.
            *   **Event:** Wait for `PaymentSucceeded` or `PaymentFailed` event.
            *   **If `PaymentSucceeded`:**
                *   **Activity:** Notify success (e.g., via SignalR), potentially publish `BookingConfirmed` event.
                *   **Transition:** Move to `Confirmed` (final) state.
            *   **If `PaymentFailed`:**
                *   **Activity:** Initiate compensation - Send `ReleaseRoom` command.
                *   **Transition:** Move to `PaymentFailed` state, then potentially `RoomReleaseRequested`.
                *   **Event:** Wait for `RoomReleased`.
                *   **Transition:** Move to `Failed` (final) state.
        6.  **If `RoomHoldFailed`:**
            *   **Activity:** Notify failure (e.g., via SignalR).
            *   **Transition:** Move to `Failed` (final) state.
    *   **Compensation:** The Saga automatically handles sending compensating commands (`ReleaseRoom`) if steps fail after preceding steps succeeded.
*   **Message Contracts (Commands/Events):** Defined likely in a shared library or within the Application/Domain layers (e.g., `HoldRoom`, `RoomHeld`, `ProcessPayment`, `PaymentSucceeded`, `ReleaseRoom`, `BookingRequested`, `BookingConfirmed`, `BookingFailed`).
*   **Simulated Consumers:** Simple MassTransit consumers that receive `HoldRoom` and `ProcessPayment` commands and respond with success/failure events after a short delay to mimic external service interaction.
*   **SignalR Integration:** The Saga state machine, during key transitions or activities, will use an injected `ISignalRNotifier` (implemented in Infrastructure using `IHubContext`) to push status updates (e.g., `BookingStatusUpdated` message containing `BookingId` and `Status`) to the connected frontend clients via the `BookingStatusHub`.

This overview provides a solid foundation for understanding the structure and flow of the `BookingService` before proceeding with the actual code implementation.
