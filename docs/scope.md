# 2. High-Level System Scope and Features

### 2.1. System Purpose
The Hotel Booking System is a backend platform designed to manage the complete, multi-step process of reserving a hotel room. Its primary purpose is to serve as a robust, reliable, and scalable engine that coordinates inventory checks and payment processing to confirm a booking. It is built on a microservices architecture to ensure resilience and separation of concerns, using the Saga pattern to manage distributed transactions.

### 2.2. In Scope
The initial scope of the system is tightly focused on the backend orchestration of a booking.
-   **Booking Orchestration:** The system will accept a booking request containing all necessary information (e.g., Room Type ID, Hotel ID, Dates, Guest Info, Total Price). It will then orchestrate the required steps to confirm this booking.
-   **Asynchronous Processing:** The system will process bookings asynchronously. A user will receive an immediate acknowledgment that their request has been accepted for processing, not a final confirmation.
-   **Status Tracking:** The system will provide an endpoint to query the real-time status of a booking as it moves through the saga (e.g., `RoomHoldRequested`, `PaymentProcessing`, `Confirmed`, `Failed`).
-   **Inventory Holds:** The system will interact with a dedicated inventory service to place and release temporary holds on rooms. The inventory service is responsible for preventing overbooking.
-   **Payment Processing:** The system will interact with a dedicated payment service to process charges. This service will simulate interactions with an external payment provider.
-   **Failure and Compensation:** The system will gracefully handle failures at any step of the process. For example, if payment fails after a room has been held, the system will automatically issue a command to release the room hold, ensuring data consistency.

### 2.3. Out of Scope
To ensure focus and a manageable initial delivery, the following features are explicitly out of scope for this version:
-   **User Interface (UI):** No frontend or user-facing interface will be developed. The system will be interacted with via its API endpoints.
-   **User Accounts and Authentication:** The system will not manage user accounts, profiles, or authentication/authorization. Requests are treated as anonymous and are identified only by the data within the request (e.g., guest email).
-   **Hotel and Room Management:** There will be no functionality for adding, editing, or managing hotel properties or room type details (e.g., descriptions, photos, amenities). This data is assumed to exist and is referenced by ID.
-   **Search and Pricing Calculation:** The system will not provide functionality to search for available rooms or calculate booking prices. The client is expected to provide the final `TotalPrice` in the booking request.
-   **Booking Modifications and Cancellations:** The ability for a user to modify or cancel a *confirmed* booking is not included.
-   **Notifications:** No email, SMS, or push notifications will be sent to the user. Status must be checked manually via the API.
-   **Multi-Currency and Localization:** The system will operate with a single, hardcoded currency (e.g., USD) and language (English). 