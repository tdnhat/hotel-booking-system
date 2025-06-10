# Backend API Specification: Hotel Booking System

## 1. Overview

This document specifies the REST API endpoints for the Hotel Booking System backend microservices. The system exposes APIs through two primary services: RoomManagementService and BookingService.

## 2. General API Guidelines

### 2.1. Base URLs

- **RoomManagementService**: `https://localhost:{port}/`
- **BookingService**: `https://localhost:{port}/`
- **Service Discovery**: Managed through .NET Aspire

### 2.2. Common Response Formats

#### Success Response
```json
{
  "data": {},
  "timestamp": "2024-01-20T10:30:00Z"
}
```

#### Error Response
```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human readable error message",
    "details": "Additional error details"
  },
  "timestamp": "2024-01-20T10:30:00Z"
}
```

### 2.3. HTTP Status Codes

- `200 OK` - Successful GET request
- `201 Created` - Successful POST request
- `204 No Content` - Successful DELETE request
- `400 Bad Request` - Invalid request parameters
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### 2.4. Content Types

- **Request**: `application/json`
- **Response**: `application/json`

## 3. RoomManagementService API

### 3.1. Get All Room Types

**Endpoint**: `GET /rooms/roomTypes`

**Description**: Retrieves all room types from all hotels

**Parameters**: 
- `hotelId` (query, optional): Filter by specific hotel ID

**Implementation Status**: ✅ Implemented

#### Request Examples

```http
GET /rooms/roomTypes
Accept: application/json
```

```http
GET /rooms/roomTypes?hotelId=123e4567-e89b-12d3-a456-426614174000
Accept: application/json
```

#### Response

**Success (200 OK)**:
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "hotelId": "987fcdeb-51a2-43d1-9c3f-123456789abc",
    "name": "Deluxe Suite",
    "description": "Spacious suite with city view and modern amenities",
    "pricePerNight": 299.99,
    "maxGuests": 4,
    "totalRooms": 10,
    "availableRooms": 7
  },
  {
    "id": "456e7890-e89b-12d3-a456-426614174001",
    "hotelId": "987fcdeb-51a2-43d1-9c3f-123456789abc",
    "name": "Standard Room",
    "description": "Comfortable room with essential amenities",
    "pricePerNight": 149.99,
    "maxGuests": 2,
    "totalRooms": 20,
    "availableRooms": 15
  }
]
```

**Error (404 Not Found)**:
```json
{
  "error": {
    "code": "ROOM_TYPES_NOT_FOUND",
    "message": "No room types found"
  }
}
```

### 3.2. Get Room Type by ID

**Endpoint**: `GET /rooms/roomTypes/{id}`

**Description**: Retrieves a specific room type by its unique identifier

**Parameters**:
- `id` (path, required): Room type UUID

**Implementation Status**: ✅ Implemented

#### Request Example

```http
GET /rooms/roomTypes/123e4567-e89b-12d3-a456-426614174000
Accept: application/json
```

#### Response

**Success (200 OK)**:
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "hotelId": "987fcdeb-51a2-43d1-9c3f-123456789abc",
  "name": "Deluxe Suite",
  "description": "Spacious suite with city view and modern amenities",
  "pricePerNight": 299.99,
  "maxGuests": 4,
  "totalRooms": 10,
  "availableRooms": 7,
  "features": [
    "Free WiFi",
    "Air Conditioning",
    "Mini Bar",
    "City View"
  ]
}
```

**Error (404 Not Found)**:
```json
{
  "error": {
    "code": "ROOM_TYPE_NOT_FOUND",
    "message": "Room type with specified ID not found"
  }
}
```

### 3.3. Planned Endpoints

#### Check Room Availability

**Endpoint**: `GET /rooms/roomTypes/{id}/availability`

**Description**: Check availability for a specific room type within a date range

**Parameters**:
- `id` (path, required): Room type UUID
- `checkInDate` (query, required): Check-in date (ISO 8601)
- `checkOutDate` (query, required): Check-out date (ISO 8601)
- `guests` (query, optional): Number of guests

**Implementation Status**: 🚧 Planned

#### Request Example

```http
GET /rooms/roomTypes/123e4567-e89b-12d3-a456-426614174000/availability?checkInDate=2024-03-15&checkOutDate=2024-03-18&guests=2
Accept: application/json
```

#### Response

```json
{
  "roomTypeId": "123e4567-e89b-12d3-a456-426614174000",
  "available": true,
  "availableRooms": 5,
  "totalPrice": 899.97,
  "priceBreakdown": {
    "basePrice": 299.99,
    "nights": 3,
    "taxes": 89.99,
    "fees": 10.00
  }
}
```

## 4. BookingService API

### 4.1. Create Booking

**Endpoint**: `POST /api/bookings`

**Description**: Initiates a new booking request using the Saga pattern

**Implementation Status**: 🚧 Planned

#### Request Example

```http
POST /api/bookings
Content-Type: application/json

{
  "roomTypeId": "123e4567-e89b-12d3-a456-426614174000",
  "hotelId": "987fcdeb-51a2-43d1-9c3f-123456789abc",
  "guestEmail": "guest@example.com",
  "checkInDate": "2024-03-15",
  "checkOutDate": "2024-03-18",
  "numberOfGuests": 2,
  "guestDetails": {
    "firstName": "John",
    "lastName": "Doe",
    "phone": "+1234567890"
  },
  "paymentDetails": {
    "cardNumber": "****-****-****-1234",
    "expiryMonth": 12,
    "expiryYear": 2025
  }
}
```

#### Response

**Success (202 Accepted)**:
```json
{
  "bookingId": "booking-123e4567-e89b-12d3-a456-426614174000",
  "correlationId": "saga-456e7890-e89b-12d3-a456-426614174001",
  "status": "Submitted",
  "message": "Booking request has been submitted and is being processed",
  "estimatedProcessingTime": "30-60 seconds"
}
```

**Error (400 Bad Request)**:
```json
{
  "error": {
    "code": "INVALID_BOOKING_REQUEST",
    "message": "Invalid booking request",
    "details": {
      "checkInDate": "Check-in date must be in the future",
      "numberOfGuests": "Number of guests exceeds room capacity"
    }
  }
}
```

### 4.2. Get Booking Status

**Endpoint**: `GET /api/bookings/{bookingId}/status`

**Description**: Retrieves the current status of a booking request

**Parameters**:
- `bookingId` (path, required): Booking identifier

**Implementation Status**: 🚧 Planned

#### Request Example

```http
GET /api/bookings/booking-123e4567-e89b-12d3-a456-426614174000/status
Accept: application/json
```

#### Response

**Success (200 OK)**:
```json
{
  "bookingId": "booking-123e4567-e89b-12d3-a456-426614174000",
  "correlationId": "saga-456e7890-e89b-12d3-a456-426614174001",
  "status": "PaymentProcessing",
  "currentStep": "ProcessingPayment",
  "progress": {
    "completed": ["RoomHeld"],
    "current": "PaymentProcessing",
    "remaining": ["BookingConfirmation"]
  },
  "lastUpdated": "2024-01-20T10:32:15Z",
  "estimatedCompletion": "2024-01-20T10:33:00Z"
}
```

**Possible Status Values**:
- `Submitted` - Initial state
- `RoomHoldRequested` - Requesting room hold
- `RoomHeld` - Room successfully held
- `PaymentProcessing` - Processing payment
- `Confirmed` - Booking confirmed
- `Failed` - Booking failed
- `Cancelled` - Booking cancelled

### 4.3. Get Booking Details

**Endpoint**: `GET /api/bookings/{bookingId}`

**Description**: Retrieves complete booking details

**Implementation Status**: 🚧 Planned

#### Response

```json
{
  "bookingId": "booking-123e4567-e89b-12d3-a456-426614174000",
  "status": "Confirmed",
  "roomType": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Deluxe Suite",
    "hotelId": "987fcdeb-51a2-43d1-9c3f-123456789abc"
  },
  "guest": {
    "email": "guest@example.com",
    "firstName": "John",
    "lastName": "Doe"
  },
  "stayDetails": {
    "checkInDate": "2024-03-15",
    "checkOutDate": "2024-03-18",
    "numberOfGuests": 2,
    "numberOfNights": 3
  },
  "pricing": {
    "totalAmount": 899.97,
    "basePrice": 799.98,
    "taxes": 89.99,
    "fees": 10.00
  },
  "confirmationNumber": "CONF-ABC123",
  "createdAt": "2024-01-20T10:30:00Z",
  "confirmedAt": "2024-01-20T10:32:45Z"
}
```

## 5. Real-time API (SignalR)

### 5.1. Booking Status Hub

**Hub URL**: `/hubs/booking-status`

**Description**: Real-time updates for booking status changes

**Implementation Status**: 🚧 Planned

#### Connection

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/booking-status")
    .build();

await connection.start();
```

#### Join Booking Group

```javascript
await connection.invoke("JoinBookingGroup", bookingId);
```

#### Listen for Status Updates

```javascript
connection.on("BookingStatusUpdated", (update) => {
    console.log("Status update:", update);
});
```

#### Status Update Message Format

```json
{
  "bookingId": "booking-123e4567-e89b-12d3-a456-426614174000",
  "status": "PaymentProcessing",
  "message": "Processing payment for your booking",
  "timestamp": "2024-01-20T10:32:15Z",
  "progress": 60
}
```

## 6. Health Check Endpoints

### 6.1. Service Health

**Endpoints**: 
- `GET /health` - Overall service health
- `GET /alive` - Liveness check

**Implementation Status**: ✅ Implemented

#### Response

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.045",
  "entries": {
    "self": {
      "data": {},
      "status": "Healthy",
      "duration": "00:00:00.001"
    },
    "database": {
      "data": {},
      "status": "Healthy",
      "duration": "00:00:00.044"
    }
  }
}
```

## 7. Error Handling

### 7.1. Common Error Codes

| Code | Description |
|------|-------------|
| `ROOM_TYPE_NOT_FOUND` | Specified room type does not exist |
| `ROOM_TYPES_NOT_FOUND` | No room types found for the query |
| `INVALID_BOOKING_REQUEST` | Booking request validation failed |
| `BOOKING_NOT_FOUND` | Specified booking does not exist |
| `ROOM_UNAVAILABLE` | Requested room is not available |
| `PAYMENT_FAILED` | Payment processing failed |
| `INTERNAL_ERROR` | Internal server error |

### 7.2. Validation Errors

Validation errors return detailed field-level information:

```json
{
  "error": {
    "code": "VALIDATION_FAILED",
    "message": "Request validation failed",
    "details": {
      "roomTypeId": "Room type ID is required",
      "checkInDate": "Check-in date must be in the future",
      "guestEmail": "Valid email address is required"
    }
  }
}
```

## 8. API Versioning

### 8.1. Version Strategy

- **Current Version**: v1
- **Versioning Method**: URL path versioning
- **Base Path**: `/api/v1/`

### 8.2. Version Headers

```http
API-Version: 1.0
Supported-Versions: 1.0
```

## 9. Rate Limiting

### 9.1. Limits (Planned)

- **General Endpoints**: 100 requests per minute
- **Booking Creation**: 10 requests per minute per IP
- **Status Queries**: 60 requests per minute

### 9.2. Rate Limit Headers

```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640995200
```

This API specification provides a comprehensive guide for integrating with the Hotel Booking System backend services, supporting both current implementations and planned features. 