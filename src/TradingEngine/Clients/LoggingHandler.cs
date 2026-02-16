using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace TradingEngine.Clients;

public class LoggingHandler(ILogger<LoggingHandler> logger, IOptions<ApplicationSettings> options) : NamedDelegatingHandler
{
    private readonly ApplicationSettings _settings =  options.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // Log the request
            await LogRequest(Name, request);

            // Start timer and send request
            var stopwatch = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            // Log if predefined time threshold is exceeded
            if (stopwatch.ElapsedMilliseconds >= _settings.LongRunningRequestThresholdInMs)
            {
                LogLongRunningRequest(Name, request, stopwatch.ElapsedMilliseconds);
            }

            // Log the response with elapsed time
            await LogResponse(Name, request, response, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception e)
        {
            LogFailedRequest(Name, request, e);
            throw;
        }
    }

    private async Task LogRequest(string @interface, HttpRequestMessage request)
    {
        var requestHeaders = request.Headers.ToString();
        logger.LogDebug("[{HttpInterface:l}] Request headers: \r\n {Body}", @interface, requestHeaders);

        var requestBody = await ReadContent(request.Content);
        logger.LogDebug("[{HttpInterface:l}] Request body: \r\n {Body}", @interface, requestBody);
        
        logger.LogInformation("[{HttpInterface:l}] Request: {Method:l} {RequestUri}", @interface, request.Method, request.RequestUri);
    }


    private async Task LogResponse(string @interface, HttpRequestMessage request, HttpResponseMessage response, long elapsedMilliseconds)
    {

            //for debug purposes in non-prod environments we log the response body and full headers
            logger.LogDebug("[{HttpInterface:l}] Response headers: \r\n {Headers}", @interface, response.Headers.ToString());

            var responseBody = await ReadContent(response.Content);
            logger.LogDebug("[{HttpInterface:l}] Response body: \r\n {Body}", @interface, responseBody);

        logger.LogInformation("[{HttpInterface:l}] Response: {Method:l} {RequestUri} returned {StatusCode} in {Elapsed} ms",
                                   @interface, request.Method, request.RequestUri, (int)response.StatusCode, elapsedMilliseconds);
    }


    private void LogLongRunningRequest(string @interface, HttpRequestMessage request, long elapsedMilliseconds)
    {
        logger.LogInformation("[{HttpInterface:l}] Long running request: {Method:l} {RequestUri} returned in {Elapsed} ms",
                         @interface, request.Method, request.RequestUri, elapsedMilliseconds);
    }


    private void LogFailedRequest(string @interface, HttpRequestMessage request, Exception exception)
    {
        logger
            .LogError(exception, "[{HttpInterface:l}] Request: {Method:l} {RequestUri} failed unexpectedly",
                      @interface, request.Method, request.RequestUri);
    }


    private static async Task<string?> ReadContent(HttpContent? request)
    {
        if (request == null) return null;
        return await request.ReadAsStringAsync();
    }
}