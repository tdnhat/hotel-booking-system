# Feature List: Hotel Booking System (Initial Phases 1-3) - Multi-Hotel

This document lists the core features planned for implementation within the initial development phases (1-3) of the Hotel Booking System, derived from the project blueprint and requirement analysis. The system has been designed to support multiple hotels instead of a single hotel.

## Core Features

1.  **Multi-Hotel Room Type Display:**
    *   The system displays room types from multiple hotels fetched from the backend.
    *   Users can view all room types across hotels or filter by a specific hotel.
    *   Each listing includes essential details such as Hotel ID, Room Name, Description, Price Per Night, Max Guests, Total Rooms, and Available Rooms.
    *   **Status:** ✅ **Implemented** - API endpoints `GET /api/v1/rooms/roomTypes` and `GET /api/v1/rooms/roomTypes?hotelId={hotelId}` are available.

2.  **Individual Room Type Details:**
    *   Users can retrieve detailed information about a specific room type.
    *   **Status:** ✅ **Implemented** - API endpoint `GET /api/v1/rooms/roomTypes/{id}` is available.

3.  **Hotel-Specific Room Filtering:**
    *   Users can filter room types by specific hotel using the hotel ID parameter.
    *   The system supports both global and hotel-scoped room queries.
    *   **Status:** ✅ **Implemented** - Available via query parameter in the room types endpoint.

4.  **Booking Request Initiation (Multi-Hotel):**
    *   Guests can select a desired room type from any hotel in the system.
    *   Guests can submit a request to book the selected room type through a simple interface.
    *   **Status:** 🚧 **Planned** - To be implemented in next phase.

5.  **Asynchronous Booking Workflow (Saga):**
    *   Backend processes booking requests using a reliable, multi-step Saga pattern (via MassTransit).
    *   The workflow coordinates simulated interactions with Inventory and Payment services across multiple hotels.
    *   **Status:** 🚧 **Planned** - To be implemented.

6.  **Simulated Multi-Hotel Inventory Hold:**
    *   The Saga includes a step to request and confirm a temporary hold on the selected room type from a simulated Inventory service.
    *   Each hotel's inventory is managed separately while maintaining system-wide consistency.
    *   **Status:** 🚧 **Planned** - To be implemented.

7.  **Simulated Payment Processing:**
    *   Following a successful room hold, the Saga includes a step to request processing from a simulated Payment service.
    *   **Status:** 🚧 **Planned** - To be implemented.

8.  **Workflow Compensation (Rollback):**
    *   If a step in the Saga fails (e.g., payment simulation fails), previously completed steps (e.g., room hold) are automatically compensated/rolled back (e.g., room is released).
    *   **Status:** 🚧 **Planned** - To be implemented.

9.  **Booking Confirmation:**
    *   A booking is considered confirmed only upon successful completion of all necessary Saga steps (room hold, payment).
    *   **Status:** 🚧 **Planned** - To be implemented.

10. **Real-time Status Updates (Frontend):**
    *   The frontend UI provides guests with live updates on the status of their booking request.
    *   Status changes (e.g., "Processing", "Room Held", "Payment Pending", "Confirmed", "Failed") are pushed from the backend via SignalR and reflected immediately in the UI.
    *   **Status:** 🚧 **Planned** - To be implemented.

11. **Booking Status Query API:**
    *   A dedicated backend API endpoint allows querying the current status of a specific booking using its unique ID.
    *   **Status:** 🚧 **Planned** - To be implemented.

12. **Basic Frontend Structure (Multi-Hotel):**
    *   A React application provides the user interface, including components for listing hotels, filtering room types by hotel, displaying room details, and submitting booking requests.
    *   Client-side routing manages navigation between different views (hotel selection, room browsing, booking process).
    *   Frontend state management (using Zustand) handles UI state related to the booking process and hotel selection.
    *   **Status:** 🚧 **Planned** - To be implemented.

13. **Clean Architecture Backend:**
    *   The backend .NET solution is structured according to Clean Architecture principles, separating concerns into Domain, Application, Infrastructure, and API layers.
    *   **Status:** ✅ **Implemented** - Room Management Service follows Clean Architecture.

14. **Multi-Hotel Database Schema:**
    *   The database schema supports multiple hotels with proper relationships.
    *   Room types are associated with hotels via `HotelId` foreign key.
    *   Database is pre-populated with sample room type data for multiple hotels for development and testing purposes.
    *   **Status:** ✅ **Implemented** - Database schema supports multi-hotel with seeded data.

## Multi-Hotel Specific Features

15. **Hotel Context Management:**
    *   System maintains proper hotel context throughout the booking process.
    *   All room-related operations are scoped to the appropriate hotel.
    *   **Status:** ✅ **Implemented** - Room types include hotel associations.

16. **Cross-Hotel Search Capabilities:**
    *   Users can search for room types across all hotels or within specific hotels.
    *   **Status:** ✅ **Implemented** - Supported via API filtering.

17. **Hotel-Specific Business Rules:**
    *   The architecture allows for different hotels to have different pricing, availability, and business rules.
    *   **Status:** ✅ **Partially Implemented** - Foundation is in place for hotel-specific customization.

## Implementation Status Legend

*   ✅ **Implemented** - Feature is fully implemented and functional
*   🚧 **Planned** - Feature is designed but not yet implemented
*   ⏸️ **Out of Scope** - Feature is not planned for initial phases

This feature list summarizes the tangible capabilities the multi-hotel system will possess upon completion of the planned initial phases, with clear indication of what has been implemented versus what is planned.
