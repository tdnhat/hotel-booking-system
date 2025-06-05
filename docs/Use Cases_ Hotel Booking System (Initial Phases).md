# Use Cases: Hotel Booking System (Initial Phases)

This document outlines the primary use cases for the Hotel Booking System during its initial development phases (1-3), focusing on the guest perspective.

## Actors

*   **Guest:** An individual visiting the website to potentially book a hotel room.

## Use Cases

### UC01: View Available Room Types

*   **Actor:** Guest
*   **Goal:** To see the list of available room types offered by the hotel, along with their prices.
*   **Preconditions:** The Guest has navigated to the hotel booking website.
*   **Main Success Scenario:**
    1.  The Guest accesses the main page of the booking application.
    2.  The system retrieves the list of available room types (including name, price per night, available count) from the backend API (`GET /api/rooms`).
    3.  The system displays the room types clearly to the Guest, potentially using cards or a list format.
*   **Postconditions:** The Guest can view the available room options.
*   **Alternative Flows:**
    *   3a. If the system cannot retrieve room types (e.g., API error), it displays an appropriate error message to the Guest.

### UC02: Request a Room Booking

*   **Actor:** Guest
*   **Goal:** To initiate the booking process for a selected room type.
*   **Preconditions:**
    1.  The Guest is viewing the available room types (UC01 completed).
    2.  The Guest has identified a room type they wish to book.
*   **Main Success Scenario:**
    1.  The Guest selects a specific room type.
    2.  The system presents a simple booking form (details required might be minimal initially, e.g., just confirming the choice).
    3.  The Guest confirms their intention to book (e.g., clicks a "Book Now" or "Request Booking" button).
    4.  The system sends a booking request to the backend API (`POST /api/bookings`) containing necessary details (e.g., selected RoomTypeId, potentially a UserId if implemented later).
    5.  The backend API accepts the request, initiates the asynchronous booking Saga, and returns an immediate response (e.g., 202 Accepted with a unique `bookingId`).
    6.  The system informs the Guest that their booking request has been submitted and is being processed, providing the `bookingId` for reference.
*   **Postconditions:** The booking request is submitted to the backend Saga for processing. The Guest is aware the request is in progress.
*   **Alternative Flows:**
    *   4a. If the booking form validation fails (if any implemented), the system prompts the Guest to correct the input.
    *   5a. If the backend API fails to accept the request (e.g., server error), the system displays an error message to the Guest.

### UC03: Track Booking Status in Real-Time

*   **Actor:** Guest
*   **Goal:** To monitor the progress of their submitted booking request.
*   **Preconditions:** The Guest has successfully submitted a booking request (UC02 completed) and received a `bookingId`.
*   **Main Success Scenario:**
    1.  After submitting the booking request, the system's UI indicates the booking is "In Progress".
    2.  The system establishes a real-time connection (SignalR) to the backend, associated with the Guest's session or `bookingId`.
    3.  As the backend Saga progresses through its states (e.g., Submitted -> RoomHeld -> PaymentProcessing -> Confirmed/Failed), it pushes status updates via SignalR.
    4.  The system's UI receives these real-time updates and displays the current status to the Guest (e.g., "Securing your room...", "Processing payment...", "Booking Confirmed!", "Booking Failed: Room unavailable/Payment issue").
*   **Postconditions:** The Guest is informed of the final outcome (confirmation or failure) and intermediate steps of their booking request in real-time.
*   **Alternative Flows:**
    *   2a. (Initial Implementation - Phase 2): If the real-time connection is not yet implemented, the system periodically polls the status API (`GET /api/bookings/{bookingId}/status`) to retrieve and display the current status.
    *   2b. If the real-time connection fails, the system may fall back to polling or display an error message indicating status updates might be delayed.

These use cases cover the core guest interactions defined within the scope of the initial project phases.
