using DntConsole.DataLayer.Context;
using DntConsole.Entities;
using DntConsole.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DntConsole.Services;

/// <summary>
///     This is a sample service to show
///     - How to inject different services
///     And
///     - How to use the EF-Core in a console app correctly
/// </summary>
public class BlogPostsService : IBlogPostsService
{
    private readonly AppHttpClientService _appHttpClientService;
    private readonly IAppDbContextFactory _contextFactory;
    private readonly ILogger<BlogPostsService> _logger;
    private readonly ISecurityService _securityService;

    public BlogPostsService(IAppDbContextFactory contextFactory,
                            ISecurityService securityService,
                            AppHttpClientService appHttpClientService,
                            ILogger<BlogPostsService> logger)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _appHttpClientService = appHttpClientService ?? throw new ArgumentNullException(nameof(appHttpClientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task AddUrlAsync(string blogPostUrl)
    {
        await using var context = _contextFactory.CreateScopedDbContext();
        var urlHash = _securityService.GetSha256Hash(blogPostUrl);

        if (await context.BlogPosts.AnyAsync(blogPost => blogPost.UrlHash == urlHash))
        {
            _logger.LogWarning("`{BlogPostUrl}` is duplicate.", blogPostUrl);
            return;
        }

        var content = await _appHttpClientService.DownloadPageAsync(blogPostUrl);

        context.BlogPosts.Add(new BlogPost { Url = blogPostUrl, UrlHash = urlHash, Content = content });
        await context.SaveChangesAsync();

        _logger.LogDebug("{BlogPostUrl} has been added.", blogPostUrl);
    }

    public async Task<bool> IsUrlAddedAsync(string blogPostUrl)
    {
        await using var context = _contextFactory.CreateScopedDbContext();
        var urlHash = _securityService.GetSha256Hash(blogPostUrl);
        return await context.BlogPosts.AnyAsync(blogPost => blogPost.UrlHash == urlHash);
    }
}