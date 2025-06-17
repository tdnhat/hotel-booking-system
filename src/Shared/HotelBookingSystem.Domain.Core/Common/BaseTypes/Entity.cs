namespace HotelBookingSystem.Domain.Core.Common.BaseTypes
{
    public abstract class Entity<TId> where TId : notnull
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public TId Id { get; protected set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected Entity() { }

        protected Entity(TId id)
        {
            Id = id;
        }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> entity || GetType() != obj.GetType())
                return false;

            return Id.Equals(entity.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
