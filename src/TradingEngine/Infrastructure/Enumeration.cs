using System.Reflection;

namespace TradingEngine.Infrastructure;

public abstract class Enumeration<T> : ValueObject, IComparable where T : Enumeration<T>, new()
{
    private static int _currentId = 0;
    public int Id { get; init; }
    public string Value { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;

    protected Enumeration() {}
    
    protected Enumeration(string value, string? displayName = null)
    {
        Id = Interlocked.Increment(ref _currentId);;
        Value = value;
        DisplayName = displayName ?? value;
    }
    
    protected Enumeration(int id, string value, string? displayName = null)
    {
        Id = id;
        Value = value;
        DisplayName = displayName ?? value;
    }

    public override string ToString()
    {
        return DisplayName;
    }

    private static IEnumerable<T> GetAll()
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (var info in fields)
        {
            var instance = new T();

            if (info.GetValue(instance) is T locatedValue)
            {
                yield return locatedValue;
            }
        }
    }
        
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration<T> otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Value.Equals(otherValue.Value);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static T FromId(int id)
    {
        var matchingItem = Parse(id, "id", item => item.Id == id);
        return matchingItem;
    }

    public static T FromValue(string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
    {
        var matchingItem = Parse(value, "value", item => string.Equals(item.Value, value, comparison));
        return matchingItem;
    }

    public static bool TryParse(string value, out T? result, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
    {
        result = GetAll().FirstOrDefault(item => string.Equals(item.Value, value, comparison));
        return result is not null;
    }

    public static T FromDisplayName(string displayName, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
    {
        var matchingItem = Parse(displayName, "display name", item => string.Equals(item.DisplayName, displayName, comparison));
        return matchingItem;
    }

    private static T Parse<TK>(TK value, string description, Func<T, bool> predicate)
    {
        var matchingItem = GetAll().FirstOrDefault<T>(predicate);
        if (matchingItem != null)
            return matchingItem;

        var message = $"'{value}' is not a valid {description} for {typeof(T).Name}";
        throw new Exception(message);
    }

    public int CompareTo(object? obj)
    {
        return Id.CompareTo(((Enumeration<T>)obj!)!.Id);
    }

    public static implicit operator string(Enumeration<T> obj) => obj.Value;
}