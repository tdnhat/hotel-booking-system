# High-Level System Scope and Features: Hotel Booking System

This document defines the scope and key features for the initial development phases (1-3) of the Hotel Booking System, based on the project blueprint and stakeholder requirements.

## 1. System Overview

The system is a web application designed to allow guests to view available hotel room types and initiate a booking request. The core of the system lies in its backend, which uses a Saga pattern orchestrated by MassTransit and RabbitMQ to reliably manage the multi-step booking process (inventory check, payment processing). Real-time updates on booking status are provided to the user via SignalR.

## 2. In Scope (Phases 1-3)

The following functionalities and components are considered **IN SCOPE** for the initial development effort:

*   **Backend Development:**
    *   .NET Solution setup following Clean Architecture principles.
    *   API endpoints for retrieving room types (`GET /api/rooms`) and initiating bookings (`POST /api/bookings`).
    *   API endpoint for checking booking status (`GET /api/bookings/{bookingId}/status`).
    *   Database setup (PostgreSQL or SQL Server Express) using Entity Framework Core.
    *   Definition and seeding of `RoomType` entity.
    *   Implementation of the Booking Saga using MassTransit (`BookingSagaStateMachine`, `BookingState`).
    *   Definition of events and commands for the Saga workflow (e.g., `BookingRequested`, `HoldRoom`, `RoomHeld`, `ProcessPayment`, `PaymentSucceeded`, `ReleaseRoom`).
    *   Creation of simulated consumers for Inventory (`HoldRoomConsumer`) and Payment (`ProcessPaymentConsumer`).
    *   Integration of RabbitMQ for messaging.
    *   Integration of SignalR (`BookingStatusHub`) for real-time status updates pushed from the Saga.
*   **Frontend Development:**
    *   React (TypeScript, Vite) application setup.
    *   Basic UI components: `RoomList`, `RoomCard`, `BookingForm`.
    *   Routing setup (React Router).
    *   API integration using Axios to fetch rooms and submit booking requests.
    *   Frontend state management using Zustand (`bookingStore`).
    *   Implementation of real-time status updates using the SignalR client library (replacing initial polling mechanism).
*   **Core Workflow:**
    *   Asynchronous, Saga-based booking process orchestration.
    *   Handling of success and failure paths, including compensating actions (rollback).

## 3. Out of Scope (Phases 1-3)

The following functionalities are explicitly **OUT OF SCOPE** for the initial development phases:

*   User authentication and authorization.
*   User account management (registration, login, profile).
*   Actual payment gateway integration (payment process will be simulated).
*   Actual inventory management system integration (inventory check will be simulated).
*   Hotel staff/administrator interface (e.g., managing room types, viewing all bookings, managing users).
*   Complex search and filtering functionality for rooms (e.g., by date, occupancy).
*   Multi-language support.
*   Email or SMS notifications.
*   Detailed UI/UX design beyond basic functional components.
*   Deployment and hosting infrastructure setup (focus is on local development environment).
*   Reporting and analytics.

## 4. Key Features Summary

Based on the functional requirements, the key features delivered by the end of Phase 3 will be:

*   **Room Browsing:** Guests can view a list of available room types with basic details.
*   **Booking Initiation:** Guests can select a room and submit a booking request.
*   **Asynchronous Booking Processing:** Backend handles booking requests via a reliable, multi-step Saga workflow.
*   **Simulated Inventory Hold:** The system simulates checking and holding room availability.
*   **Simulated Payment Processing:** The system simulates the payment step.
*   **Saga-based Rollback:** Failed steps trigger automatic compensation for preceding successful steps.
*   **Real-time Booking Status Updates:** Guests receive live updates on their booking progress directly in the UI.
*   **Status Check API:** An endpoint allows querying the current state of a specific booking.

This scope definition provides a clear boundary for the initial development effort, focusing on implementing the core Saga pattern and real-time updates as outlined in the blueprint.
