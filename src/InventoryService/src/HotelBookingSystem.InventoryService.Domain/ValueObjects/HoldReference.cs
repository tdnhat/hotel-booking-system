namespace HotelBookingSystem.InventoryService.Domain.ValueObjects;

/// <summary>
/// Represents a unique reference for a room hold.
/// Value object that ensures uniqueness and proper formatting.
/// </summary>
public class HoldReference : IEquatable<HoldReference>
{
    /// <summary>
    /// Gets the string value of the hold reference.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HoldReference"/> class.
    /// </summary>
    /// <param name="value">The hold reference value.</param>
    private HoldReference(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new hold reference with the specified value.
    /// </summary>
    /// <param name="value">The hold reference value.</param>
    /// <returns>A new HoldReference instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is invalid.</exception>
    public static HoldReference Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Hold reference cannot be null or empty.", nameof(value));

        if (value.Length < 8 || value.Length > 50)
            throw new ArgumentException("Hold reference must be between 8 and 50 characters.", nameof(value));

        if (!IsValidFormat(value))
            throw new ArgumentException("Hold reference contains invalid characters.", nameof(value));

        return new HoldReference(value);
    }

    /// <summary>
    /// Generates a new unique hold reference.
    /// </summary>
    /// <param name="prefix">Optional prefix for the hold reference (default: "HLD").</param>
    /// <returns>A new HoldReference with a unique value.</returns>
    public static HoldReference Generate(string prefix = "HLD")
    {
        if (string.IsNullOrWhiteSpace(prefix))
            prefix = "HLD";

        // Generate a unique reference using timestamp and random component
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var reference = $"{prefix}-{timestamp}-{random}";

        return new HoldReference(reference);
    }

    /// <summary>
    /// Validates the format of a hold reference.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns>True if the format is valid, false otherwise.</returns>
    private static bool IsValidFormat(string value)
    {
        // Allow alphanumeric characters, hyphens, and underscores
        return value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is HoldReference other && Equals(other);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(HoldReference? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Implicit conversion from HoldReference to string.
    /// </summary>
    /// <param name="holdReference">The hold reference to convert.</param>
    public static implicit operator string(HoldReference holdReference)
    {
        return holdReference?.Value ?? string.Empty;
    }

    /// <summary>
    /// Determines whether two hold references are equal.
    /// </summary>
    /// <param name="left">The first hold reference.</param>
    /// <param name="right">The second hold reference.</param>
    /// <returns>True if the hold references are equal; otherwise, false.</returns>
    public static bool operator ==(HoldReference? left, HoldReference? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two hold references are not equal.
    /// </summary>
    /// <param name="left">The first hold reference.</param>
    /// <param name="right">The second hold reference.</param>
    /// <returns>True if the hold references are not equal; otherwise, false.</returns>
    public static bool operator !=(HoldReference? left, HoldReference? right)
    {
        return !Equals(left, right);
    }
} 