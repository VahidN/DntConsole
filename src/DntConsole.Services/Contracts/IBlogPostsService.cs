namespace DntConsole.Services.Contracts;

public interface IBlogPostsService
{
    Task AddUrlAsync(string blogPostUrl);

    Task<bool> IsUrlAddedAsync(string blogPostUrl);
}