using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class SportEventId(Guid value) : ValueObject
{
    private Guid Value { get; } = value;

    public static SportEventId New => new(Guid.NewGuid());

    public static implicit operator string(SportEventId obj) => obj.Value.ToString();

    public static explicit operator SportEventId(string s) => new(Guid.Parse(s));
    
    public override string ToString()
    {
        return Value.ToString();
    }
}