using DntConsole.DataLayer.Context;
using DntConsole.Services;
using DntConsole.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DntConsole.IoCConfig;

public static class ConfigureServices
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppRunnerService, AppRunnerService>();
        services.AddSingleton<ISecurityService, SecurityService>();
        services.AddSingleton<IAppDbContextFactory, AppDbContextFactory>();
        services.AddSingleton<IBlogPostsService, BlogPostsService>();
    }
}