using DntConsole.Models;
using DntConsole.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;

namespace DntConsole.IoCConfig;

public static class ConfigureHttpClient
{
    private static readonly HttpClientHandler HttpClientHandler = new()
                                                                  {
                                                                      UseProxy = false,
                                                                      Proxy = null,
                                                                      AllowAutoRedirect = true,
                                                                      AutomaticDecompression = DecompressionMethods.All,
                                                                  };

    static ConfigureHttpClient() =>
        // Increases the concurrent outbound connections
        ServicePointManager.DefaultConnectionLimit = 1024;

    public static void AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.Get<AppConfig>() ?? throw new InvalidOperationException("config is null");
        var builder = services
                      .AddHttpClient<AppHttpClientService>(client => client.ConfigureClientDefaults(config))
                      .AddTransientHttpErrorPolicy(policy =>
                                                       // transient errors: network failures and HTTP 5xx and HTTP 408 errors
                                                       policy.WaitAndRetryAsync(new[]
                                                                                    {
                                                                                        TimeSpan.FromSeconds(3),
                                                                                        TimeSpan.FromSeconds(5),
                                                                                        TimeSpan.FromSeconds(15),
                                                                                    }));
        builder.ConfigurePrimaryHttpMessageHandler(() => HttpClientHandler);
        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); // Remove logging of the HttpClient
    }

    private static void ConfigureClientDefaults(this HttpClient client, AppConfig config)
    {
        client.BaseAddress = new Uri(config.HttpClientConfig.BaseAddress);
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.Add("User-Agent", config.HttpClientConfig.UserAgent);
        client.DefaultRequestHeaders.Add("Accept",
                                         "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        client.DefaultRequestHeaders.Referrer = new Uri(config.HttpClientConfig.Referrer);
    }
}