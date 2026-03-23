using System.Collections.Concurrent;
using System.Reflection;

namespace TradingEngine.Infrastructure;

public abstract class ValueObject
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> TypeProperties = new();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;
        if (GetType() != obj.GetType()) return false;
        var other = obj as ValueObject;
        return other != null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return GetEqualityComponents().Aggregate(17, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
        }
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"{{{string.Join(", ", GetFields().Select(f => $"{f.Name}: {f.GetValue(this)}"))}}}";
    }

    protected virtual IEnumerable<object?> GetEqualityComponents()
    {
        return GetFields().Select(f => f.GetValue(this));
    }

    private IEnumerable<FieldInfo> GetFields()
    {
        return GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .OrderBy(f => f.Name);
    }
}

