namespace HotelBookingSystem.InventoryService.Domain.Exceptions;

public class InventoryException : Exception
{
    public InventoryException(string message) : base(message)
    {
    }

    public InventoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class InsufficientInventoryException : InventoryException
{
    public InsufficientInventoryException(string message) : base(message)
    {
    }
}

public class HoldExpiredException : InventoryException
{
    public HoldExpiredException(string message) : base(message)
    {
    }
}

public class InvalidHoldStatusException : InventoryException
{
    public InvalidHoldStatusException(string message) : base(message)
    {
    }
}

public class HoldNotFoundException : InventoryException
{
    public HoldNotFoundException(string message) : base(message)
    {
    }
} 