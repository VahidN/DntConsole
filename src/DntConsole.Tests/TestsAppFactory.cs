using DntConsole.DataLayer.Context;
using DntConsole.IoCConfig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DntFeeds.Tests;

internal static class TestsAppFactory
{
    private static readonly Lazy<IHost> HostProvider = new(GetHost, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    ///     A lazy loaded thread-safe singleton
    /// </summary>
    public static IHost Host { get; } = HostProvider.Value;

    public static T GetRequiredService<T>() where T : notnull => Host.Services.GetRequiredService<T>();

    private static IHost GetHost()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                               .ConfigureServices((context, services) =>
                                                      services.AddAppServices(context.Configuration, true));
        var app = builder.Build();
        app.Services.GetRequiredService<IAppDbContextFactory>().EnsureDatabaseCreated();

        return app;
    }
}