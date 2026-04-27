namespace TradingEngine.Clients;

public abstract class NamedDelegatingHandler : DelegatingHandler
{
    /// <summary>
    /// Name for the target system/interface for which the http client should be used
    /// This identifier will be available to all handlers inheriting from this class which can provide context for any given operation.
    /// </summary>
    public string Name { get; set; } = "HttpClient";
}