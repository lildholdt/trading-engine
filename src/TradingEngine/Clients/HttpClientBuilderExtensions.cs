namespace TradingEngine.Clients;

public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Add a named message handler to an <see cref="IHttpClientBuilder"/>.
    /// The message handler needs to inherit from <see cref="NamedDelegatingHandler"/> and will then have access to the name of the <see cref="HttpClient"/> which can be used as a reference.
    /// </summary>
    public static IHttpClientBuilder AddNamedHttpMessageHandler<THandler>(this IHttpClientBuilder builder)
        where THandler : NamedDelegatingHandler
    {
        return builder.AddHttpMessageHandler(provider =>
        {
            var handler = provider.GetRequiredService<THandler>();
            handler.Name = builder.Name;
            return handler;
        });
    }
}