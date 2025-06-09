# Stakeholder Requirements Analysis: Hotel Booking System (Multi-Hotel)

This document outlines the initial stakeholder requirements derived from the provided project blueprint and typical expectations for a multi-hotel booking system. As the Business Analyst, I've identified the key needs for different stakeholders involved in a system that manages multiple hotels instead of a single property.

## 1. Identified Stakeholders

*   **Hotel Chain Management/Client:** The primary business stakeholder interested in a functional, reliable multi-hotel booking system.
*   **Individual Hotel Owners/Managers:** Stakeholders responsible for specific hotel properties within the system.
*   **Guests (End Users):** Individuals using the system to browse and book rooms across multiple hotels.
*   **Developer (You):** Responsible for implementing the system according to the blueprint and requirements.
*   **System Maintainers (Implicit):** Future team responsible for operating and maintaining the multi-hotel system.

## 2. High-Level Business Goals

*   Provide a seamless online booking experience for guests across multiple hotel properties.
*   Ensure reliable handling of room inventory across different hotels to prevent overbooking.
*   Implement a robust and resilient booking process capable of handling failures gracefully across hotel boundaries.
*   Lay a foundation for a scalable and maintainable multi-hotel system architecture.
*   Enable hotel-specific business rules while maintaining system-wide consistency.
*   Support future expansion to additional hotels without significant architectural changes.

## 3. Functional Requirements (What the system must do)

*   **FR01:** Display available room types from multiple hotels to users, including details like hotel ID, name, description, price per night, max guests, total rooms, and available rooms (derived from `GET /api/v1/rooms/roomTypes` and multi-hotel `RoomType` entity).
    *   **Status:** ✅ **Implemented**

*   **FR02:** Allow users to filter room types by specific hotel (derived from `GET /api/v1/rooms/roomTypes?hotelId={hotelId}`).
    *   **Status:** ✅ **Implemented**

*   **FR03:** Provide detailed information for individual room types with hotel context (derived from `GET /api/v1/rooms/roomTypes/{id}`).
    *   **Status:** ✅ **Implemented**

*   **FR04:** Allow users to select a room type from any hotel and initiate a booking request (derived from UI components and future `POST /api/bookings`).
    *   **Status:** 🚧 **Planned**

*   **FR05:** Process booking requests asynchronously using a Saga pattern to coordinate multiple steps with hotel-specific context (core concept of the blueprint).
    *   **Status:** 🚧 **Planned**

*   **FR06:** Interact with hotel-specific Inventory systems/services (simulated) to temporarily hold rooms upon request (`HoldRoom` command with hotel context).
    *   **Status:** 🚧 **Planned**

*   **FR07:** Interact with Payment systems/services (simulated) to process payment after a room is held (`ProcessPayment` command).
    *   **Status:** 🚧 **Planned**

*   **FR08:** Confirm bookings only if both room hold and payment are successful, with proper hotel attribution (`BookingConfirmed` event).
    *   **Status:** 🚧 **Planned**

*   **FR09:** Implement compensating actions (rollback) if any step in the booking process fails, ensuring hotel-specific inventory is properly released (e.g., release room hold if payment fails - `ReleaseRoom` command).
    *   **Status:** 🚧 **Planned**

*   **FR10:** Provide users with real-time feedback on the status of their booking request with hotel context (e.g., Submitted, Room Held at [Hotel], Payment Processing, Confirmed, Failed) (derived from SignalR integration).
    *   **Status:** 🚧 **Planned**

*   **FR11:** Store booking details and the current state of the booking Saga with hotel associations (derived from future `BookingState` entity).
    *   **Status:** 🚧 **Planned**

*   **FR12:** Provide an API endpoint for users to check the status of a specific booking (`GET /api/bookings/{bookingId}/status`).
    *   **Status:** 🚧 **Planned**

*   **FR13:** The system must be pre-populated with initial room type data for multiple hotels for demonstration and testing (derived from seeding requirement).
    *   **Status:** ✅ **Implemented**

*   **FR14:** Support hotel-scoped room type management ensuring room types are properly associated with their respective hotels via `HotelId`.
    *   **Status:** ✅ **Implemented**

## 4. Non-Functional Requirements (How the system must perform)

*   **NFR01: Reliability:** The booking workflow must be highly reliable across multiple hotels, ensuring data consistency even if parts of the process fail (addressed by Saga pattern with hotel context).

*   **NFR02: Responsiveness:** The user interface must provide immediate feedback upon booking submission and real-time updates on status changes, including hotel-specific information (addressed by SignalR).

*   **NFR03: Maintainability:** The system codebase must follow Clean Architecture principles for ease of understanding and modification across hotel-specific and system-wide concerns.

*   **NFR04: Scalability:** The underlying architecture (event-driven, potential microservices) should be designed to handle future growth in number of hotels, bookings, and load (implied by RabbitMQ/MassTransit).

*   **NFR05: Usability:** The guest-facing UI must be simple and intuitive for browsing hotels, filtering room types, and initiating bookings across multiple properties.

*   **NFR06: Isolation:** Hotel-specific data and operations must be properly isolated while maintaining system-wide consistency and avoiding cross-hotel data leakage.

*   **NFR07: Extensibility:** The system architecture must allow for easy addition of new hotels and hotel-specific business rules without affecting existing hotels.

## 5. Technical Constraints & Considerations (From Blueprint + Multi-Hotel)

*   **TC01:** Backend technology stack: .NET (ASP.NET Core), EF Core with multi-hotel data model.

*   **TC02:** Frontend technology stack: React (TypeScript, Vite), Zustand, Axios with multi-hotel state management.

*   **TC03:** Messaging infrastructure: RabbitMQ with MassTransit for Saga orchestration across hotel boundaries.

*   **TC04:** Real-time communication: SignalR with hotel-specific context.

*   **TC05:** Database: PostgreSQL with multi-hotel schema design.

*   **TC06:** Architecture: Clean Architecture with proper separation of hotel-specific and system-wide concerns.

*   **TC07:** Data Model: All room-related entities must include hotel associations via `HotelId` foreign keys.

## 6. Multi-Hotel Specific Requirements

*   **MH01:** Each hotel must be able to have different room types, pricing, and availability without affecting other hotels.

*   **MH02:** The system must support querying room types globally or filtered by specific hotels.

*   **MH03:** Booking processes must maintain hotel context throughout the entire workflow.

*   **MH04:** Hotel-specific business rules must be enforceable while maintaining system-wide consistency.

*   **MH05:** The system must be designed to easily onboard new hotels without major architectural changes.

## 7. Assumptions

*   Authentication and user management are out of scope for Phase 1-3.
*   Payment processing and inventory management are simulated initially.
*   Detailed UI design specifics are flexible beyond the basic components mentioned.
*   Hotel management administrative functions (adding hotels, managing cross-hotel operations) are out of scope for this initial phase.
*   All hotels in the initial implementation share the same basic business rules and processes.
*   Hotel-specific customizations will be addressed in future phases.

## 8. Implementation Status

*   ✅ **Completed:** Multi-hotel data model, room type APIs with hotel filtering
*   🚧 **In Progress:** Clean Architecture implementation for Room Management Service
*   🚧 **Planned:** Booking Saga implementation, Frontend development, SignalR integration

This analysis forms the basis for the more detailed requirement documents (Use Cases, User Stories, Feature List) and reflects the evolution from single hotel to multi-hotel system requirements.
