# Use Cases: Hotel Booking System (Initial Phases) - Multi-Hotel

This document outlines the primary use cases for the Hotel Booking System during its initial development phases (1-3), focusing on the guest perspective. The system has been updated to support multiple hotels instead of a single hotel.

## Actors

*   **Guest:** An individual visiting the website to potentially book hotel rooms from multiple properties.
*   **Hotel Manager:** A person responsible for managing room types and availability for a specific hotel (future scope).

## Use Cases

### UC01: View Available Room Types Across Hotels

*   **Actor:** Guest
*   **Goal:** To see the list of available room types offered by multiple hotels, along with their prices and hotel-specific information.
*   **Preconditions:** The Guest has navigated to the hotel booking website.
*   **Main Success Scenario:**
    1.  The Guest accesses the main page of the booking application.
    2.  The system retrieves the list of available room types from all hotels (including hotel ID, name, description, price per night, max guests, total rooms, available count) from the backend API (`GET /api/v1/rooms/roomTypes`).
    3.  The system displays the room types clearly to the Guest, potentially using cards or a list format, with hotel identification for each room type.
    4.  **(Optional)** The Guest can filter room types by a specific hotel using a hotel selector or filter option.
    5.  **(Optional)** The system retrieves room types for the selected hotel (`GET /api/v1/rooms/roomTypes?hotelId={hotelId}`) and updates the display.
*   **Postconditions:** The Guest can view the available room options from multiple hotels, with the ability to filter by specific hotel.
*   **Alternative Flows:**
    *   3a. If the system cannot retrieve room types (e.g., API error), it displays an appropriate error message to the Guest.
    *   5a. If filtering by hotel fails, the system falls back to showing all room types and displays an error message.
*   **Implementation Status:** ✅ **Implemented** - API endpoints are functional.

### UC01.1: View Specific Room Type Details

*   **Actor:** Guest
*   **Goal:** To view detailed information about a specific room type, including its hotel association.
*   **Preconditions:** The Guest is viewing the available room types (UC01 completed) and has identified a room type of interest.
*   **Main Success Scenario:**
    1.  The Guest selects a specific room type from the list.
    2.  The system retrieves detailed information for the room type from the backend API (`GET /api/v1/rooms/roomTypes/{id}`).
    3.  The system displays comprehensive room type details including hotel ID, full description, amenities, pricing, and availability information.
*   **Postconditions:** The Guest has detailed information about the selected room type and its associated hotel.
*   **Alternative Flows:**
    *   2a. If the room type details cannot be retrieved (e.g., room type not found), the system displays an appropriate error message.
*   **Implementation Status:** ✅ **Implemented** - API endpoint is functional.

### UC02: Request a Room Booking (Multi-Hotel)

*   **Actor:** Guest
*   **Goal:** To initiate the booking process for a selected room type from any hotel in the system.
*   **Preconditions:**
    1.  The Guest is viewing the available room types (UC01 completed).
    2.  The Guest has identified a room type they wish to book from a specific hotel.
*   **Main Success Scenario:**
    1.  The Guest selects a specific room type from a specific hotel.
    2.  The system presents a booking form that includes hotel information and room type details (details required might be minimal initially, e.g., just confirming the choice and hotel).
    3.  The Guest confirms their intention to book (e.g., clicks a "Book Now" or "Request Booking" button).
    4.  The system sends a booking request to the backend API (`POST /api/bookings`) containing necessary details (e.g., selected RoomTypeId, HotelId, potentially a UserId if implemented later).
    5.  The backend API accepts the request, initiates the asynchronous booking Saga with hotel context, and returns an immediate response (e.g., 202 Accepted with a unique `bookingId`).
    6.  The system informs the Guest that their booking request has been submitted for the specific hotel and is being processed, providing the `bookingId` for reference.
*   **Postconditions:** The booking request is submitted to the backend Saga for processing with proper hotel context. The Guest is aware the request is in progress for the selected hotel.
*   **Alternative Flows:**
    *   4a. If the booking form validation fails (if any implemented), the system prompts the Guest to correct the input.
    *   5a. If the backend API fails to accept the request (e.g., server error), the system displays an error message to the Guest.
*   **Implementation Status:** 🚧 **Planned** - To be implemented.

### UC03: Track Booking Status in Real-Time (Multi-Hotel Context)

*   **Actor:** Guest
*   **Goal:** To monitor the progress of their submitted booking request with awareness of which hotel the booking is for.
*   **Preconditions:** The Guest has successfully submitted a booking request (UC02 completed) and received a `bookingId`.
*   **Main Success Scenario:**
    1.  After submitting the booking request, the system's UI indicates the booking is "In Progress" and displays the hotel information.
    2.  The system establishes a real-time connection (SignalR) to the backend, associated with the Guest's session or `bookingId`.
    3.  As the backend Saga progresses through its states (e.g., Submitted -> RoomHeld -> PaymentProcessing -> Confirmed/Failed), it pushes status updates via SignalR with hotel-specific context.
    4.  The system's UI receives these real-time updates and displays the current status to the Guest with hotel context (e.g., "Securing your room at [Hotel Name]...", "Processing payment...", "Booking Confirmed at [Hotel Name]!", "Booking Failed at [Hotel Name]: Room unavailable/Payment issue").
*   **Postconditions:** The Guest is informed of the final outcome (confirmation or failure) and intermediate steps of their booking request in real-time with proper hotel context.
*   **Alternative Flows:**
    *   2a. (Initial Implementation - Phase 2): If the real-time connection is not yet implemented, the system periodically polls the status API (`GET /api/bookings/{bookingId}/status`) to retrieve and display the current status.
    *   2b. If the real-time connection fails, the system may fall back to polling or display an error message indicating status updates might be delayed.
*   **Implementation Status:** 🚧 **Planned** - To be implemented.

### UC04: Browse Hotels and Their Room Types (Future Enhancement)

*   **Actor:** Guest
*   **Goal:** To browse hotels first and then view room types for selected hotels.
*   **Preconditions:** The Guest has navigated to the hotel booking website.
*   **Main Success Scenario:**
    1.  The Guest accesses a hotel listing page.
    2.  The system displays available hotels with basic information (name, location, etc.).
    3.  The Guest selects a specific hotel.
    4.  The system navigates to the room types for the selected hotel (using UC01 with hotel filter).
*   **Postconditions:** The Guest can browse hotels and view their respective room types.
*   **Implementation Status:** 🚧 **Future Enhancement** - Not in current scope.

## Multi-Hotel Specific Use Cases

### UC05: Manage Hotel-Specific Room Types (Out of Scope)

*   **Actor:** Hotel Manager
*   **Goal:** To manage room types for their specific hotel without affecting other hotels.
*   **Preconditions:** Hotel Manager is authenticated and authorized for their specific hotel.
*   **Main Success Scenario:**
    1.  Hotel Manager accesses their hotel management interface.
    2.  System displays room types scoped to their hotel only.
    3.  Manager can add, edit, or remove room types for their hotel.
    4.  All operations are scoped to their hotel via `HotelId` constraint.
*   **Postconditions:** Hotel's room types are updated without affecting other hotels.
*   **Implementation Status:** ⏸️ **Out of Scope** - Not planned for initial phases.

## Implementation Status Legend

*   ✅ **Implemented** - Use case is fully supported by implemented features
*   🚧 **Planned** - Use case is designed but implementation is pending
*   🚧 **Future Enhancement** - Use case is identified for future implementation beyond initial phases
*   ⏸️ **Out of Scope** - Use case is not planned for initial phases

These use cases cover the core guest interactions defined within the scope of the initial project phases, updated to reflect the multi-hotel architecture and current implementation status.
