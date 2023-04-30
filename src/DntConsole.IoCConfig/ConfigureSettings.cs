using DntConsole.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DntConsole.IoCConfig;

public static class ConfigureSettings
{
    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppConfig>(configuration.Bind);
    }
}
