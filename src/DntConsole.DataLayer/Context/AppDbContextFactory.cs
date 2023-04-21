using DntConsole.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DntConsole.DataLayer.Context;

public class AppDbContextFactory : IAppDbContextFactory
{
    private readonly IOptions<AppConfig> _appConfig;
    private readonly IHostEnvironment _hostingEnvironment;
    private readonly ILogger<AppDbContextFactory> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AppDbContextFactory(IOptions<AppConfig> appConfig,
                               IHostEnvironment hostingEnvironment,
                               IServiceProvider serviceProvider,
                               ILogger<AppDbContextFactory> logger)
    {
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetConnectionString()
    {
        var connectionString = _appConfig.Value.ConnectionStrings.ApplicationDbContextConnection;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("connectionString is null");
        }

        if (connectionString.Contains("%CONTENTROOTPATH%", StringComparison.OrdinalIgnoreCase))
        {
            var basePath = _hostingEnvironment.ContentRootPath;
            var contentRootPath = basePath.Split(new[] { "\\bin\\" }, StringSplitOptions.RemoveEmptyEntries)[0];
            _logger.LogDebug("Content Root Path: `{ContentRootPath}`", contentRootPath);
            connectionString = connectionString.Replace("%CONTENTROOTPATH%",
                                                        contentRootPath,
                                                        StringComparison.OrdinalIgnoreCase);
        }

        _logger.LogDebug("Connection String: `{ConnectionString}`", connectionString);
        return connectionString;
    }

    public async Task InitDatabaseAsync()
    {
        await using var context = CreateScopedDbContext();
        await context.Database.MigrateAsync();
    }

    public AppDbContext CreateScopedDbContext()
    {
        var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        return serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public void EnsureDatabaseCreated()
    {
        using var context = CreateScopedDbContext();
        context.Database.EnsureCreated();
    }
}