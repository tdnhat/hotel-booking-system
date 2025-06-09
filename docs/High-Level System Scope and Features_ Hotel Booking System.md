# High-Level System Scope and Features: Hotel Booking System (Multi-Hotel)

This document defines the scope and key features for the initial development phases (1-3) of the Hotel Booking System, based on the project blueprint and stakeholder requirements. The system has evolved to support multiple hotels instead of a single hotel.

## 1. System Overview

The system is a web application designed to allow guests to browse available room types across multiple hotels and initiate booking requests. The core of the system lies in its backend, which uses a Saga pattern orchestrated by MassTransit and RabbitMQ to reliably manage the multi-step booking process (inventory check, payment processing). Real-time updates on booking status are provided to the user via SignalR.

## 2. In Scope (Phases 1-3)

The following functionalities and components are considered **IN SCOPE** for the initial development effort:

*   **Backend Development:**
    *   .NET Solution setup following Clean Architecture principles.
    *   API endpoints for retrieving room types across all hotels or filtered by specific hotel (`GET /api/v1/rooms/roomTypes`, `GET /api/v1/rooms/roomTypes?hotelId={hotelId}`).
    *   API endpoint for retrieving specific room type details (`GET /api/v1/rooms/roomTypes/{id}`).
    *   API endpoints for initiating bookings (`POST /api/bookings`) - to be implemented.
    *   API endpoint for checking booking status (`GET /api/bookings/{bookingId}/status`) - to be implemented.
    *   Database setup (PostgreSQL) using Entity Framework Core with multi-hotel support.
    *   Definition and seeding of `RoomType` entity with hotel associations.
    *   Implementation of the Booking Saga using MassTransit (`BookingSagaStateMachine`, `BookingState`) - to be implemented.
    *   Definition of events and commands for the Saga workflow (e.g., `BookingRequested`, `HoldRoom`, `RoomHeld`, `ProcessPayment`, `PaymentSucceeded`, `ReleaseRoom`) - to be implemented.
    *   Creation of simulated consumers for Inventory (`HoldRoomConsumer`) and Payment (`ProcessPaymentConsumer`) - to be implemented.
    *   Integration of RabbitMQ for messaging - to be implemented.
    *   Integration of SignalR (`BookingStatusHub`) for real-time status updates pushed from the Saga - to be implemented.
*   **Frontend Development:**
    *   React (TypeScript, Vite) application setup - to be implemented.
    *   Basic UI components: `HotelList`, `RoomList`, `RoomCard`, `BookingForm` - to be implemented.
    *   Hotel selection and filtering capabilities - to be implemented.
    *   Routing setup (React Router) - to be implemented.
    *   API integration using Axios to fetch hotels, rooms and submit booking requests - to be implemented.
    *   Frontend state management using Zustand (`bookingStore`, `hotelStore`) - to be implemented.
    *   Implementation of real-time status updates using the SignalR client library - to be implemented.
*   **Core Workflow:**
    *   Asynchronous, Saga-based booking process orchestration - to be implemented.
    *   Handling of success and failure paths, including compensating actions (rollback) - to be implemented.
    *   Multi-hotel room inventory management.

## 3. Out of Scope (Phases 1-3)

The following functionalities are explicitly **OUT OF SCOPE** for the initial development phases:

*   User authentication and authorization.
*   User account management (registration, login, profile).
*   Hotel management and administration interface (adding/editing hotels, room types, etc.).
*   Actual payment gateway integration (payment process will be simulated).
*   Actual inventory management system integration (inventory check will be simulated).
*   Hotel staff/administrator interface (e.g., managing room types, viewing all bookings, managing users).
*   Complex search and filtering functionality for rooms (e.g., by date, occupancy, location, amenities).
*   Multi-language support.
*   Email or SMS notifications.
*   Detailed UI/UX design beyond basic functional components.
*   Deployment and hosting infrastructure setup (focus is on local development environment).
*   Reporting and analytics.
*   Hotel chain management and hierarchical relationships.

## 4. Key Features Summary

Based on the functional requirements, the key features delivered by the end of Phase 3 will be:

*   **Multi-Hotel Room Browsing:** Guests can view room types from multiple hotels, with the ability to filter by specific hotel.
*   **Hotel Selection:** Guests can browse and select from multiple hotels in the system.
*   **Room Type Management:** System supports room types associated with specific hotels via `HotelId`.
*   **Booking Initiation:** Guests can select a room from any hotel and submit a booking request - to be implemented.
*   **Asynchronous Booking Processing:** Backend handles booking requests via a reliable, multi-step Saga workflow - to be implemented.
*   **Simulated Multi-Hotel Inventory Hold:** The system simulates checking and holding room availability across different hotels - to be implemented.
*   **Simulated Payment Processing:** The system simulates the payment step - to be implemented.
*   **Saga-based Rollback:** Failed steps trigger automatic compensation for preceding successful steps - to be implemented.
*   **Real-time Booking Status Updates:** Guests receive live updates on their booking progress directly in the UI - to be implemented.
*   **Status Check API:** An endpoint allows querying the current state of a specific booking - to be implemented.

## 5. Multi-Hotel Architecture Notes

The system architecture supports multiple hotels through:

*   **Hotel-scoped Room Types:** Each `RoomType` entity is associated with a specific `HotelId`.
*   **Flexible Querying:** API endpoints support both global room type queries and hotel-specific filtering.
*   **Scalable Design:** The Clean Architecture approach allows for easy extension to support hotel-specific business rules.
*   **Data Isolation:** Room types and availability are properly scoped by hotel while maintaining system-wide consistency.

This scope definition provides a clear boundary for the initial development effort, focusing on implementing the core multi-hotel room management and preparing for the Saga pattern and real-time updates as outlined in the blueprint.
