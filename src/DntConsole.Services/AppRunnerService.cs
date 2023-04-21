using CommandLine;
using DntConsole.DataLayer.Context;
using DntConsole.Models;
using DntConsole.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DntConsole.Services;

/// <summary>
///     Defines the entry point of the application
/// </summary>
public class AppRunnerService : IAppRunnerService
{
    private readonly IOptions<AppConfig> _appConfig;
    private readonly IAppDbContextFactory _appDbContextFactory;
    private readonly IBlogPostsService _blogPostsService;
    private readonly ILogger<AppRunnerService> _logger;

    public AppRunnerService(ILogger<AppRunnerService> logger,
                            IOptions<AppConfig> appConfig,
                            IAppDbContextFactory appDbContextFactory,
                            IBlogPostsService blogPostsService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        _appDbContextFactory = appDbContextFactory ??
                               throw new ArgumentNullException(nameof(appDbContextFactory));
        _blogPostsService = blogPostsService ?? throw new ArgumentNullException(nameof(blogPostsService));
    }

    public async Task StartAsync(string[] args)
    {
        try
        {
            await _appDbContextFactory.InitDatabaseAsync();

            await Parser.Default
                        .ParseArguments<AppArgs>(args)
                        .WithParsedAsync(async appArgs =>
                                         {
                                             // This is sample to show the entry point of the APP
                                             // How to use an injected service and how to process the received args
                                             if (string.Equals(appArgs.Id, "ID1", StringComparison.OrdinalIgnoreCase))
                                             {
                                                 await _blogPostsService.AddUrlAsync(
                                                  _appConfig.Value.HttpClientConfig.BaseAddress);
                                             }
                                         });
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error");
        }
    }
}