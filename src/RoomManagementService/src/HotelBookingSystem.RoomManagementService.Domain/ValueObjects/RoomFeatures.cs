using HotelBookingSystem.RoomManagementService.Domain.Common.BaseTypes;

namespace HotelBookingSystem.RoomManagementService.Domain.ValueObjects
{
    public class RoomFeatures : ValueObject
    {
        private readonly HashSet<string> _features;

        public IReadOnlyCollection<string> Features => _features.ToList().AsReadOnly();

        private RoomFeatures()
        {
            _features = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        private RoomFeatures(IEnumerable<string> features)
        {
            _features = new HashSet<string>(
                features.Where(f => !string.IsNullOrWhiteSpace(f)),
                StringComparer.OrdinalIgnoreCase);
        }

        public static RoomFeatures Empty() => new();

        public static RoomFeatures Create(params string[] features) => new(features);

        public static RoomFeatures Create(IEnumerable<string> features) => new(features);

        public RoomFeatures AddFeature(string feature)
        {
            if (string.IsNullOrWhiteSpace(feature))
                return this;

            var newFeatures = new List<string>(_features) { feature.Trim() };
            return new RoomFeatures(newFeatures);
        }

        public RoomFeatures RemoveFeature(string feature)
        {
            if (string.IsNullOrWhiteSpace(feature) || !_features.Contains(feature))
                return this;

            var newFeatures = _features.Where(f => !f.Equals(feature, StringComparison.OrdinalIgnoreCase));
            return new RoomFeatures(newFeatures);
        }

        public bool HasFeature(string feature) =>
            !string.IsNullOrWhiteSpace(feature) && _features.Contains(feature);

        public int Count => _features.Count;

        protected override IEnumerable<object> GetAtomicValues()
        {
            foreach (var feature in _features.OrderBy(f => f))
                yield return feature;
        }

        public override string ToString() => string.Join(", ", _features.OrderBy(f => f));

        // Common hotel room features as static properties
        public static RoomFeatures StandardFeatures() => Create(
            "Air Conditioning", "Free WiFi", "Private Bathroom", "Television");

        public static RoomFeatures LuxuryFeatures() => Create(
            "Air Conditioning", "Free WiFi", "Private Bathroom", "Television",
            "Mini Bar", "Room Service", "Balcony", "City View");
    }
}
