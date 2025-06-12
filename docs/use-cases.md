# 3. Use Cases and User Stories

### 3.1. Actors
-   **Guest:** An end-user (or a client system acting on their behalf) wishing to book a hotel room.
-   **System:** The collection of microservices that constitute the Hotel Booking System.

### 3.2. Use Case 1: Book a Hotel Room
-   **ID:** UC-001
-   **Use Case Name:** Book a Hotel Room
-   **Actor:** Guest
-   **Description:** A Guest submits a request to book a specific room type for a given date range. The System processes the request by verifying inventory, holding a room, and processing payment. The Guest can later check the outcome of the request.
-   **Basic Flow (Happy Path):**
    1.  Guest sends a booking request to the System's API.
    2.  System validates the request format.
    3.  System immediately responds with a "202 Accepted" status and a unique `BookingId`.
    4.  System internally initiates the booking saga.
    5.  System requests a hold on the specified room inventory. The inventory is available.
    6.  System requests payment processing. The payment is successful.
    7.  System marks the booking as `Confirmed`.
-   **Alternative Flows (Sad Paths):**
    -   **A1: Inventory Not Available:**
        -   At step 5, the System determines the room inventory is not available for the requested dates.
        -   The System marks the booking as `Failed` with a reason of "No availability".
        -   The use case ends.
    -   **A2: Payment Fails:**
        -   At step 6, the System determines the payment has failed.
        -   The System issues a compensating command to release the inventory hold from step 5.
        -   The System marks the booking as `Failed` with a reason of "Payment declined".
        -   The use case ends.

#### 3.2.1. User Stories for UC-001

-   **Story ID:** US-001
-   **Title:** Initiate a New Booking
-   **User Story:** "As a Guest, I want to submit a booking request for a specific room type and dates, so that I can reserve a room."
-   **Acceptance Criteria:**
    -   Given I provide a valid `RoomTypeId`, `HotelId`, date range, and total price,
    -   When I send a `POST` request to the `/bookings` endpoint,
    -   Then the system must respond with an HTTP `202 Accepted` status code.
    -   And the response body must contain a unique `BookingId`.
    -   And the system must publish a `BookingRequested` event internally to start the booking process.

-   **Story ID:** US-002
-   **Title:** Handle Unavailable Inventory
-   **User Story:** "As a System, when a booking request is made for an unavailable room, I want to terminate the booking process so that rooms are not overbooked."
-   **Acceptance Criteria:**
    -   Given the booking saga has started for `BookingId-123`,
    -   And the `InventoryService` determines there is no availability for the requested room and dates,
    -   When the `InventoryService` processes the `HoldRoom` command,
    -   Then it must publish a `RoomHoldFailed` event.
    -   And the booking status for `BookingId-123` must be updated to `Failed`.

-   **Story ID:** US-003
-   **Title:** Handle Payment Failure and Compensate
-   **User Story:** "As a System, when a payment fails for a a booking, I want to cancel the booking and release any held inventory so that the room becomes available for others."
-   **Acceptance Criteria:**
    -   Given the booking saga for `BookingId-456` has successfully held a room in the `InventoryService`,
    -   And the `PaymentService` determines the payment has failed,
    -   When the `PaymentService` processes the `ProcessPayment` command,
    -   Then it must publish a `PaymentFailed` event.
    -   And the booking saga must then issue a `ReleaseRoom` command.
    -   And the `InventoryService` must successfully process the `ReleaseRoom` command, making the inventory available again.
    -   And the final booking status for `BookingId-456` must be `Failed`.

### 3.3. Use Case 2: Check Booking Status
-   **ID:** UC-002
-   **Use Case Name:** Check Booking Status
-   **Actor:** Guest
-   **Description:** A Guest who has already submitted a booking request can query the system to find out its current status.
-   **Basic Flow:**
    1.  Guest sends a request to the System's API with a `BookingId`.
    2.  System retrieves the current state of the booking associated with the `BookingId`.
    3.  System responds with the booking details, including the current status (e.g., `Submitted`, `Confirmed`, `Failed`) and failure reason, if applicable.

#### 3.3.1. User Stories for UC-002

-   **Story ID:** US-004
-   **Title:** Retrieve Current Booking Status
-   **User Story:** "As a Guest, I want to check the status of my booking using its ID, so that I can know if it is confirmed, pending, or failed."
-   **Acceptance Criteria:**
    -   Given a booking with `BookingId-789` exists in the system,
    -   When I send a `GET` request to the `/bookings/BookingId-789/status` endpoint,
    -   Then the system must respond with an HTTP `200 OK` status code.
    -   And the response body must contain the `BookingId`, the current `Status` (e.g., "Confirmed"), and other relevant booking details.
    -   And if the booking failed, the response must include a `FailureReason`.
    -   And if the `BookingId` does not exist, the system must respond with an HTTP `404 Not Found`. 