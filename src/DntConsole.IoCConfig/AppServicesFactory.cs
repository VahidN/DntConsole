using DntConsole.DataLayer.Context;
using DntConsole.Models;
using DntConsole.Services;
using DntConsole.Services.Contracts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;

namespace DntConsole.IoCConfig;

public static class AppServicesFactory
{
    private static readonly HttpClientHandler HttpClientHandler = new()
                                                                  {
                                                                      UseProxy = false,
                                                                      Proxy = null,
                                                                      AllowAutoRedirect = true,
                                                                      AutomaticDecompression = DecompressionMethods.All,
                                                                  };

    public static void AddAppServices(this IServiceCollection services,
                                      IConfiguration configuration,
                                      bool isInMemoryDb = false)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Increases the concurrent outbound connections
        ServicePointManager.DefaultConnectionLimit = 1024;
        AddHttpClient(services, configuration);
        AddSettings(services, configuration);
        AddCustomServices(services);
        AddDbContext(services, isInMemoryDb);
    }

    private static void AddDbContext(this IServiceCollection services, bool isInMemoryDb)
    {
        services.AddDbContextPool<AppDbContext>((serviceProvider, optionsBuilder) =>
                                                {
                                                    if (isInMemoryDb)
                                                    {
                                                        var builder = new SqliteConnectionStringBuilder
                                                                      {
                                                                          DataSource = $"{Guid.NewGuid()}.db",
                                                                          Mode = SqliteOpenMode.Memory,
                                                                          Cache = SqliteCacheMode.Shared,
                                                                      };
                                                        var connection = new SqliteConnection(builder.ConnectionString);
                                                        connection.Open();
                                                        connection.EnableExtensions();
                                                        optionsBuilder.UseSqlite(connection);
                                                    }
                                                    else
                                                    {
                                                        var connectionString = serviceProvider
                                                                               .GetRequiredService<
                                                                                   IAppDbContextFactory>()
                                                                               .GetConnectionString();
                                                        optionsBuilder.UseSqlite(connectionString);
                                                    }
                                                });
    }

    private static void AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppRunnerService, AppRunnerService>();
        services.AddSingleton<ISecurityService, SecurityService>();
        services.AddSingleton<IAppDbContextFactory, AppDbContextFactory>();
        services.AddSingleton<IBlogPostsService, BlogPostsService>();
    }

    private static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppConfig>(options => configuration.Bind(options));
    }

    private static void AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.Get<AppConfig>() ?? throw new InvalidOperationException("config is null");
        var builder = services.AddHttpClient<AppHttpClientService>(client =>
                                                                   {
                                                                       client.BaseAddress =
                                                                           new Uri(config.HttpClientConfig.BaseAddress);
                                                                       client.Timeout = Timeout.InfiniteTimeSpan;
                                                                       client.DefaultRequestHeaders
                                                                             .Add("User-Agent",
                                                                              config.HttpClientConfig.UserAgent);
                                                                       client.DefaultRequestHeaders
                                                                             .Add("Accept",
                                                                              "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                                                                       client.DefaultRequestHeaders
                                                                             .Add("Accept-Language", "en-US,en;q=0.5");
                                                                       client.DefaultRequestHeaders
                                                                             .Add("Accept-Encoding",
                                                                              "gzip, deflate, br");
                                                                       client.DefaultRequestHeaders.Referrer =
                                                                           new Uri(config.HttpClientConfig.Referrer);
                                                                   })
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
}