# 1. Feature List

This list captures the high-level capabilities of the system, categorized by their primary business domain.

### 1.1. Core Booking Features
-   **F-01:** Initiate a new booking request for a hotel room.
-   **F-02:** Provide a unique booking identifier upon request submission.
-   **F-03:** Track the end-to-end status of a booking (e.g., Submitted, Confirmed, Failed).
-   **F-04:** Retrieve the current status of a booking using its identifier.
-   **F-05:** Orchestrate the booking process across multiple internal services.

### 1.2. Inventory Management Features (Backend)
-   **F-06:** Process requests to place a temporary hold on room inventory for a specific date range.
-   **F-s07:** Process requests to release a temporary hold on room inventory (compensation).
-   **F-08:** Ensure room inventory cannot be double-booked.
-   **F-09:** (Future) Convert a temporary hold into a permanent reservation upon booking confirmation.

### 1.3. Payment Processing Features (Backend)
-   **F-10:** Process payment requests for a specific booking amount.
-   **F-11:** Log all payment transaction attempts and their outcomes.
-   **F-12:** Integrate with a (mocked) external payment gateway. 