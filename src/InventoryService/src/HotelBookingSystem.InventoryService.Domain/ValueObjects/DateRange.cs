using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.Domain.Core.Common.Exceptions;

namespace HotelBookingSystem.InventoryService.Domain.ValueObjects
{
    public class DateRange : ValueObject
    {
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }

        private DateRange() { }

        public DateRange(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
                throw new ArgumentException("Check-in date must be before check-out date");

            if (checkIn.Date < DateTime.Today)
                throw new ArgumentException("Check-in date cannot be in the past");

            CheckIn = checkIn.Date;
            CheckOut = checkOut.Date;
        }

        public int Nights => (CheckOut - CheckIn).Days;

        public IEnumerable<DateTime> GetDates()
        {
            for (var date = CheckIn; date < CheckOut; date = date.AddDays(1))
            {
                yield return date;
            }
        }

        public bool Contains(DateTime date)
        {
            return date.Date >= CheckIn && date.Date < CheckOut;
        }

        public bool OverlapsWith(DateRange other)
        {
            return CheckIn < other.CheckOut && CheckOut > other.CheckIn;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CheckIn;
            yield return CheckOut;
        }

        public override string ToString() => $"{CheckIn:yyyy-MM-dd} to {CheckOut:yyyy-MM-dd} ({Nights} nights)";
    }
}
