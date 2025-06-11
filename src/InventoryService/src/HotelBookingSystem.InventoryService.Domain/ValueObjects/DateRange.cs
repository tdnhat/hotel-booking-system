namespace HotelBookingSystem.InventoryService.Domain.ValueObjects;

/// <summary>
/// Represents a date range with start and end dates.
/// Value object that ensures valid date ranges for room bookings.
/// </summary>
public class DateRange : IEquatable<DateRange>
{
    /// <summary>
    /// Gets the start date of the range.
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Gets the end date of the range.
    /// </summary>
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateRange"/> class.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    private DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Creates a new date range with the specified start and end dates.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A new DateRange instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the date range is invalid.</exception>
    public static DateRange Create(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date.", nameof(startDate));

        if (startDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Start date cannot be in the past.", nameof(startDate));

        // Normalize to dates only (remove time component)
        var normalizedStart = startDate.Date;
        var normalizedEnd = endDate.Date;

        return new DateRange(normalizedStart, normalizedEnd);
    }

    /// <summary>
    /// Creates a date range from check-in and check-out dates (typical hotel booking scenario).
    /// </summary>
    /// <param name="checkInDate">The check-in date.</param>
    /// <param name="checkOutDate">The check-out date.</param>
    /// <returns>A new DateRange instance.</returns>
    public static DateRange CreateForBooking(DateTime checkInDate, DateTime checkOutDate)
    {
        return Create(checkInDate, checkOutDate);
    }

    /// <summary>
    /// Gets the number of nights in this date range.
    /// </summary>
    /// <returns>The number of nights between start and end date.</returns>
    public int GetNights()
    {
        return (EndDate - StartDate).Days;
    }

    /// <summary>
    /// Gets all dates within this range (inclusive of start, exclusive of end).
    /// </summary>
    /// <returns>An enumerable of all dates in the range.</returns>
    public IEnumerable<DateTime> GetDatesInRange()
    {
        for (var date = StartDate; date < EndDate; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    /// <summary>
    /// Checks if the specified date falls within this range.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is within the range, false otherwise.</returns>
    public bool Contains(DateTime date)
    {
        var normalizedDate = date.Date;
        return normalizedDate >= StartDate && normalizedDate < EndDate;
    }

    /// <summary>
    /// Checks if this date range overlaps with another date range.
    /// </summary>
    /// <param name="other">The other date range to check against.</param>
    /// <returns>True if the ranges overlap, false otherwise.</returns>
    public bool OverlapsWith(DateRange other)
    {
        if (other == null)
            return false;

        return StartDate < other.EndDate && EndDate > other.StartDate;
    }

    /// <summary>
    /// Extends the date range by the specified number of days.
    /// </summary>
    /// <param name="days">The number of days to extend (positive to extend end date, negative to extend start date).</param>
    /// <returns>A new DateRange with the extended duration.</returns>
    public DateRange ExtendBy(int days)
    {
        if (days > 0)
        {
            return new DateRange(StartDate, EndDate.AddDays(days));
        }
        else if (days < 0)
        {
            var newStartDate = StartDate.AddDays(days);
            if (newStartDate >= EndDate)
                throw new ArgumentException("Extension would result in invalid date range.", nameof(days));
            
            return new DateRange(newStartDate, EndDate);
        }

        return this; // No change if days is 0
    }

    /// <summary>
    /// Validates the date range for business rules.
    /// </summary>
    /// <param name="maxAdvanceBookingDays">Maximum days in advance a booking can be made (default: 365).</param>
    /// <param name="maxStayDuration">Maximum number of nights for a stay (default: 30).</param>
    /// <returns>A list of validation errors, empty if valid.</returns>
    public List<string> Validate(int maxAdvanceBookingDays = 365, int maxStayDuration = 30)
    {
        var errors = new List<string>();

        if (StartDate >= EndDate)
            errors.Add("Check-in date must be before check-out date.");

        if (StartDate.Date < DateTime.UtcNow.Date)
            errors.Add("Check-in date cannot be in the past.");

        if (StartDate.Date > DateTime.UtcNow.Date.AddDays(maxAdvanceBookingDays))
            errors.Add($"Check-in date cannot be more than {maxAdvanceBookingDays} days in advance.");

        var nights = GetNights();
        if (nights > maxStayDuration)
            errors.Add($"Stay duration cannot exceed {maxStayDuration} nights.");

        if (nights < 1)
            errors.Add("Stay must be at least one night.");

        return errors;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is DateRange other && Equals(other);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(DateRange? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartDate.Equals(other.StartDate) && EndDate.Equals(other.EndDate);
    }

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(StartDate, EndDate);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} ({GetNights()} nights)";
    }

    /// <summary>
    /// Determines whether two date ranges are equal.
    /// </summary>
    /// <param name="left">The first date range.</param>
    /// <param name="right">The second date range.</param>
    /// <returns>True if the date ranges are equal; otherwise, false.</returns>
    public static bool operator ==(DateRange? left, DateRange? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two date ranges are not equal.
    /// </summary>
    /// <param name="left">The first date range.</param>
    /// <param name="right">The second date range.</param>
    /// <returns>True if the date ranges are not equal; otherwise, false.</returns>
    public static bool operator !=(DateRange? left, DateRange? right)
    {
        return !Equals(left, right);
    }
} 