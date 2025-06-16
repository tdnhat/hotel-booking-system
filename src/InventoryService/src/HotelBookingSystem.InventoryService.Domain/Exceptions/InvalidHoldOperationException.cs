namespace HotelBookingSystem.InventoryService.Domain.Exceptions;

public class InvalidHoldOperationException : Exception
{
    public InvalidHoldOperationException() { }

    public InvalidHoldOperationException(string message) : base(message) { }

    public InvalidHoldOperationException(string message, Exception innerException) : base(message, innerException) { }
} 