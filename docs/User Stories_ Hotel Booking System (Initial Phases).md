# User Stories: Hotel Booking System (Initial Phases) - Multi-Hotel

This document presents user stories derived from the requirements and use cases for the initial phases (1-3) of the Hotel Booking System. The system has been updated to support multiple hotels instead of a single hotel.

## Guest Persona

*   **Persona:** Alex, a potential guest looking for hotel rooms online across multiple properties.
*   **Goal:** To easily find and book a suitable room from various hotels for an upcoming trip.

## User Stories

1.  **Story:** As Alex, I want to see a list of available room types from multiple hotels with their prices per night and hotel information, so that I can compare options across different properties and choose one that fits my budget and needs.
    *   *Acceptance Criteria:* The main page displays room types fetched from multiple hotels. Each room type shows its hotel ID, name, description, price, max guests, total rooms, and available rooms count.
    *   *Related Use Case:* UC01: View Available Room Types Across Hotels
    *   *Implementation Status:* ✅ **Implemented** - API endpoint `GET /api/v1/rooms/roomTypes` is available.

2.  **Story:** As Alex, I want to filter room types by a specific hotel, so that I can focus on options from a particular property that I prefer or that is conveniently located.
    *   *Acceptance Criteria:* I can filter the room type list to show only rooms from a specific hotel using a hotel selection or filter option. The API supports hotel-specific queries.
    *   *Related Use Case:* UC01: View Available Room Types Across Hotels
    *   *Implementation Status:* ✅ **Implemented** - API endpoint `GET /api/v1/rooms/roomTypes?hotelId={hotelId}` is available.

3.  **Story:** As Alex, I want to view detailed information about a specific room type, so that I can understand exactly what I'm booking and make an informed decision.
    *   *Acceptance Criteria:* I can click on a room type to view its complete details including hotel association, full description, pricing, and availability information.
    *   *Related Use Case:* UC01: View Available Room Types Across Hotels
    *   *Implementation Status:* ✅ **Implemented** - API endpoint `GET /api/v1/rooms/roomTypes/{id}` is available.

4.  **Story:** As Alex, I want to select a room type from any hotel and initiate the booking process easily, so that I can reserve the room I want without hassle, regardless of which hotel it belongs to.
    *   *Acceptance Criteria:* Clicking on a room type or a related button leads to a booking confirmation step/form. Submitting the form sends a request to the backend that handles multi-hotel bookings.
    *   *Related Use Case:* UC02: Request a Room Booking (Multi-Hotel)
    *   *Implementation Status:* 🚧 **Planned** - To be implemented in next phase.

5.  **Story:** As Alex, after requesting a booking for a room from any hotel, I want to receive immediate confirmation that my request is being processed, so that I know the system has received it and which hotel the booking is for.
    *   *Acceptance Criteria:* Upon submitting the booking form, the UI displays a message like "Booking request submitted for [Hotel Name], processing..." and provides a booking ID.
    *   *Related Use Case:* UC02: Request a Room Booking (Multi-Hotel)
    *   *Implementation Status:* 🚧 **Planned** - To be implemented.

6.  **Story:** As Alex, I want to see the real-time status of my booking request (e.g., checking availability, processing payment, confirmed, failed) with hotel-specific context, so that I am kept informed about the progress without needing to manually check.
    *   *Acceptance Criteria:* The UI updates automatically via SignalR to reflect the current stage of the booking Saga (e.g., "Room Held at [Hotel Name]", "Payment Processing", "Confirmed", "Failed").
    *   *Related Use Case:* UC03: Track Booking Status in Real-Time
    *   *Implementation Status:* 🚧 **Planned** - To be implemented.

7.  **Story:** As Alex, if my booking fails (e.g., room becomes unavailable, payment fails), I want to be clearly informed of the failure and the reason with hotel context, so that I understand what happened and can try again or choose another hotel/room option.
    *   *Acceptance Criteria:* The real-time status update clearly indicates failure and provides a simple reason (e.g., "Booking Failed at [Hotel Name]: Room Unavailable", "Booking Failed: Payment Declined").
    *   *Related Use Case:* UC03: Track Booking Status in Real-Time
    *   *Implementation Status:* 🚧 **Planned** - To be implemented.

8.  **Story:** As Alex, I want to easily switch between viewing all available rooms across all hotels and viewing rooms from a specific hotel, so that I can explore my options flexibly.
    *   *Acceptance Criteria:* The interface provides clear options to view "All Hotels" or select a specific hotel, with the room listings updating accordingly.
    *   *Related Use Case:* UC01: View Available Room Types Across Hotels
    *   *Implementation Status:* ✅ **Backend Implemented** - API supports both modes, frontend to be implemented.

## Hotel Management Persona (Future Scope)

*   **Persona:** Hotel Manager, Sarah
*   **Goal:** To manage room types and availability for her specific hotel within the multi-hotel system.

9.  **Story:** As Sarah, I want to manage room types for my specific hotel without affecting other hotels, so that I can maintain accurate inventory and pricing for my property.
    *   *Acceptance Criteria:* Hotel-specific management interface that scopes all operations to my hotel only.
    *   *Implementation Status:* ⏸️ **Out of Scope** - Not planned for initial phases.

## Developer Persona (Implicit)

*   **Persona:** The Developer (You)
*   **Goal:** To build a robust, maintainable, and scalable multi-hotel system according to the blueprint.

10. **Story:** As the Developer, I want the backend architecture to follow Clean Architecture principles with proper multi-hotel support, so that the codebase is organized, testable, and maintainable while handling multiple hotels correctly.
    *   *Acceptance Criteria:* Solution structure includes Domain, Application, Infrastructure, and API projects with appropriate dependencies. Room types are properly associated with hotels via `HotelId`.
    *   *Implementation Status:* ✅ **Implemented** - Room Management Service follows Clean Architecture with multi-hotel support.

11. **Story:** As the Developer, I want to use MassTransit and RabbitMQ to implement the booking workflow as a Saga with multi-hotel support, so that the process is resilient, handles failures gracefully (with compensation), and is decoupled across different hotels.
    *   *Acceptance Criteria:* A `BookingSagaStateMachine` orchestrates the flow using defined events and commands. Consumers handle hotel-specific tasks (HoldRoom, ProcessPayment). Compensating actions (ReleaseRoom) are defined and work correctly across hotels.
    *   *Implementation Status:* 🚧 **Planned** - To be implemented.

12. **Story:** As the Developer, I want to use SignalR to push real-time status updates from the Saga to the frontend with hotel context, so that the user experience is modern and responsive, avoiding inefficient polling.
    *   *Acceptance Criteria:* The Saga state machine uses `IHubContext` to send updates to the relevant client upon key state transitions, including hotel-specific information.
    *   *Implementation Status:* 🚧 **Planned** - To be implemented.

13. **Story:** As the Developer, I want to use Zustand for frontend state management with support for multi-hotel operations, so that I can manage UI state effectively in a simple and scalable way.
    *   *Acceptance Criteria:* Zustand stores (`bookingStore`, `hotelStore`) are implemented to hold booking status, hotel selection, errors, etc.
    *   *Implementation Status:* 🚧 **Planned** - To be implemented.

## Implementation Status Legend

*   ✅ **Implemented** - Feature is fully implemented and functional
*   ✅ **Backend Implemented** - Backend API is ready, frontend implementation pending
*   🚧 **Planned** - Feature is designed but not yet implemented
*   ⏸️ **Out of Scope** - Feature is not planned for initial phases

These user stories provide a user-centric view of the requirements for the initial development phases of the multi-hotel booking system, with clear indication of implementation status.
