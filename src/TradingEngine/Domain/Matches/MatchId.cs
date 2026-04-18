using System.Text.Json.Serialization;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.Matches;

[method: JsonConstructor]
public class MatchId(Guid value) : ValueObject
{
    public Guid Value { get; } = value;

    public static MatchId New => new(Guid.NewGuid());

    public static implicit operator string(MatchId obj) => obj.Value.ToString();
    public static implicit operator MatchId(string s) => new(Guid.Parse(s));
    public static implicit operator MatchId(Guid s) => new(s);
    
    public override string ToString()
    {
        return Value.ToString();
    }
}