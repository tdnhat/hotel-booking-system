# User Stories: Hotel Booking System (Initial Phases)

This document presents user stories derived from the requirements and use cases for the initial phases (1-3) of the Hotel Booking System.

## Guest Persona

*   **Persona:** Alex, a potential guest looking for a hotel room online.
*   **Goal:** To easily find and book a suitable room for an upcoming trip.

## User Stories

1.  **Story:** As Alex, I want to see a list of available room types with their prices per night, so that I can compare options and choose one that fits my budget and needs.
    *   *Acceptance Criteria:* The main page displays room types fetched from the backend. Each room type shows its name and price. Available count might also be shown.
    *   *Related Use Case:* UC01: View Available Room Types

2.  **Story:** As Alex, I want to select a room type and initiate the booking process easily, so that I can reserve the room I want without hassle.
    *   *Acceptance Criteria:* Clicking on a room type or a related button leads to a booking confirmation step/form. Submitting the form sends a request to the backend.
    *   *Related Use Case:* UC02: Request a Room Booking

3.  **Story:** As Alex, after requesting a booking, I want to receive immediate confirmation that my request is being processed, so that I know the system has received it.
    *   *Acceptance Criteria:* Upon submitting the booking form, the UI displays a message like "Booking request submitted, processing..." and provides a booking ID.
    *   *Related Use Case:* UC02: Request a Room Booking

4.  **Story:** As Alex, I want to see the real-time status of my booking request (e.g., checking availability, processing payment, confirmed, failed), so that I am kept informed about the progress without needing to manually check.
    *   *Acceptance Criteria:* The UI updates automatically via SignalR to reflect the current stage of the booking Saga (e.g., "Room Held", "Payment Processing", "Confirmed", "Failed").
    *   *Related Use Case:* UC03: Track Booking Status in Real-Time

5.  **Story:** As Alex, if my booking fails (e.g., room becomes unavailable, payment fails), I want to be clearly informed of the failure and the reason, so that I understand what happened and can try again or choose another option.
    *   *Acceptance Criteria:* The real-time status update clearly indicates failure and provides a simple reason (e.g., "Booking Failed: Room Unavailable", "Booking Failed: Payment Declined").
    *   *Related Use Case:* UC03: Track Booking Status in Real-Time

## Developer Persona (Implicit)

*   **Persona:** The Developer (You)
*   **Goal:** To build a robust, maintainable, and scalable system according to the blueprint.

6.  **Story:** As the Developer, I want the backend architecture to follow Clean Architecture principles, so that the codebase is organized, testable, and maintainable.
    *   *Acceptance Criteria:* Solution structure includes Domain, Application, Infrastructure, and API projects with appropriate dependencies.

7.  **Story:** As the Developer, I want to use MassTransit and RabbitMQ to implement the booking workflow as a Saga, so that the process is resilient, handles failures gracefully (with compensation), and is decoupled.
    *   *Acceptance Criteria:* A `BookingSagaStateMachine` orchestrates the flow using defined events and commands. Consumers handle specific tasks (HoldRoom, ProcessPayment). Compensating actions (ReleaseRoom) are defined.

8.  **Story:** As the Developer, I want to use SignalR to push real-time status updates from the Saga to the frontend, so that the user experience is modern and responsive, avoiding inefficient polling.
    *   *Acceptance Criteria:* The Saga state machine uses `IHubContext` to send updates to the relevant client upon key state transitions.

9.  **Story:** As the Developer, I want to use Zustand for frontend state management, so that I can manage UI state effectively in a simple and scalable way.
    *   *Acceptance Criteria:* A Zustand store (`bookingStore`) is implemented to hold booking status, errors, etc.

These user stories provide a user-centric view of the requirements for the initial development phases.
