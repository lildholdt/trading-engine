namespace TradingEngine.Infrastructure
{
    public abstract class Entity<TIdentity> : IEntity<TIdentity> where TIdentity : notnull
    {
        public TIdentity Id
        {
            get;
            init => field = value ?? throw new ArgumentNullException(nameof(Id));
        }

        public DateTime CreatedAt { get; init; }
        protected virtual object Actual => this;

        protected Entity(TIdentity id)
        {
            Id = id;
            CreatedAt = DateTime.Now;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TIdentity> other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Actual.GetType() != other.Actual.GetType())
                return false;

            return Comparer<TIdentity>.Default.Compare(Id, other.Id) >= 0;
        }

        public static bool operator ==(Entity<TIdentity>? a, Entity<TIdentity>? b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<TIdentity> a, Entity<TIdentity> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TIdentity>.Default.GetHashCode(Id);
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Id}";
        }
    }
}