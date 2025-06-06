namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public class DateRange : IEquatable<DateRange>
    {
        public DateOnly Start { get; private set; }
        public DateOnly End { get; private set; }

        // For backwards compatibility
        public DateOnly CheckIn => Start;
        public DateOnly CheckOut => End;

        public DateRange(DateOnly start, DateOnly end)
        {
            if (end <= start)
                throw new ArgumentException("End date must be after start date", nameof(end));
            
            Start = start;
            End = end;
        }

        public int NumberOfNights => (End.ToDateTime(TimeOnly.MinValue) - Start.ToDateTime(TimeOnly.MinValue)).Days;

        public bool Overlaps(DateRange other) =>
            !(this.End <= other.Start || other.End <= this.Start);

        public bool Contains(DateOnly date) =>
            date >= Start && date < End;

        public override bool Equals(object? obj) => 
            obj is DateRange other && Equals(other);

        public bool Equals(DateRange? other) =>
            other is not null && Start == other.Start && End == other.End;

        public override int GetHashCode() => 
            HashCode.Combine(Start, End);

        public override string ToString() => 
            $"{Start:yyyy-MM-dd} to {End:yyyy-MM-dd} ({NumberOfNights} nights)";

        public static bool operator ==(DateRange? left, DateRange? right) => 
            EqualityComparer<DateRange>.Default.Equals(left, right);

        public static bool operator !=(DateRange? left, DateRange? right) => 
            !(left == right);
    }
}
