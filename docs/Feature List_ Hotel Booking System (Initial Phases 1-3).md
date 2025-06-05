# Feature List: Hotel Booking System (Initial Phases 1-3)

This document lists the core features planned for implementation within the initial development phases (1-3) of the Hotel Booking System, derived from the project blueprint and requirement analysis.

## Core Features

1.  **Room Type Display:**
    *   The system displays a list of available hotel room types fetched from the backend.
    *   Each listing includes essential details such as Room Name and Price Per Night.

2.  **Booking Request Initiation:**
    *   Guests can select a desired room type.
    *   Guests can submit a request to book the selected room type through a simple interface.

3.  **Asynchronous Booking Workflow (Saga):**
    *   Backend processes booking requests using a reliable, multi-step Saga pattern (via MassTransit).
    *   The workflow coordinates simulated interactions with Inventory and Payment services.

4.  **Simulated Inventory Hold:**
    *   The Saga includes a step to request and confirm a temporary hold on the selected room type from a simulated Inventory service.

5.  **Simulated Payment Processing:**
    *   Following a successful room hold, the Saga includes a step to request processing from a simulated Payment service.

6.  **Workflow Compensation (Rollback):**
    *   If a step in the Saga fails (e.g., payment simulation fails), previously completed steps (e.g., room hold) are automatically compensated/rolled back (e.g., room is released).

7.  **Booking Confirmation:**
    *   A booking is considered confirmed only upon successful completion of all necessary Saga steps (room hold, payment).

8.  **Real-time Status Updates (Frontend):**
    *   The frontend UI provides guests with live updates on the status of their booking request.
    *   Status changes (e.g., "Processing", "Room Held", "Payment Pending", "Confirmed", "Failed") are pushed from the backend via SignalR and reflected immediately in the UI.

9.  **Booking Status Query API:**
    *   A dedicated backend API endpoint allows querying the current status of a specific booking using its unique ID.

10. **Basic Frontend Structure:**
    *   A React application provides the user interface, including components for listing rooms, displaying room details, and submitting booking requests.
    *   Client-side routing manages navigation between different views.
    *   Frontend state management (using Zustand) handles UI state related to the booking process.

11. **Clean Architecture Backend:**
    *   The backend .NET solution is structured according to Clean Architecture principles, separating concerns into Domain, Application, Infrastructure, and API layers.

12. **Database Seeding:**
    *   The database is pre-populated with sample `RoomType` data for development and testing purposes.

This feature list summarizes the tangible capabilities the system will possess upon completion of the planned initial phases.
