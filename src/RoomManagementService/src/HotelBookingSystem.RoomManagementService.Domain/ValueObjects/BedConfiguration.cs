using HotelBookingSystem.Domain.Core.Common.BaseTypes;
using HotelBookingSystem.Domain.Core.Common.Guards;
using HotelBookingSystem.RoomManagementService.Domain.Enums;

namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public class BedConfiguration : ValueObject
    {
        public BedType PrimaryBedType { get; private set; }
        public int PrimaryBedCount { get; private set; }
        public BedType SecondaryBedType { get; private set; }
        public int SecondaryBedCount { get; private set; }

        private BedConfiguration() { } // EF Core constructor

        public BedConfiguration(BedType primaryBedType, int primaryBedCount,
                               BedType secondaryBedType = BedType.None, int secondaryBedCount = 0)
        {
            PrimaryBedType = primaryBedType;
            PrimaryBedCount = Guard.Against.NegativeOrZero(primaryBedCount, nameof(primaryBedCount));
            SecondaryBedType = secondaryBedType;
            SecondaryBedCount = Math.Max(0, secondaryBedCount);

            if (SecondaryBedType == BedType.None && SecondaryBedCount > 0)
            {
                throw new ArgumentException("Secondary bed count cannot be greater than 0 when secondary bed type is None");
            }
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return PrimaryBedType;
            yield return PrimaryBedCount;
            yield return SecondaryBedType;
            yield return SecondaryBedCount;
        }

        public int CalculateMaxCapacity()
        {
            var primary = CalculateBedCapacity(PrimaryBedType, PrimaryBedCount);
            var secondary = CalculateBedCapacity(SecondaryBedType, SecondaryBedCount);
            return primary + secondary;
        }

        private static int CalculateBedCapacity(BedType bedType, int bedCount)
        {
            return bedType switch
            {
                BedType.Single => bedCount * 1,
                BedType.Double => bedCount * 2,
                BedType.Queen => bedCount * 2,
                BedType.King => bedCount * 2,
                BedType.SofaBed => bedCount * 1,
                BedType.None => 0,
                _ => 0
            };
        }

        public string GetDescription()
        {
            var parts = new List<string>();

            if (PrimaryBedCount > 0)
            {
                parts.Add($"{PrimaryBedCount} {PrimaryBedType}");
            }

            if (SecondaryBedCount > 0 && SecondaryBedType != BedType.None)
            {
                parts.Add($"{SecondaryBedCount} {SecondaryBedType}");
            }

            return string.Join(", ", parts);
        }

        public static BedConfiguration SingleBed(int count = 1) => new(BedType.Single, count);

        public static BedConfiguration DoubleBed(int count = 1) => new(BedType.Double, count);

        public static BedConfiguration QueenBed(int count = 1) => new(BedType.Queen, count);

        public static BedConfiguration KingBed(int count = 1) => new(BedType.King, count);
    }
}
