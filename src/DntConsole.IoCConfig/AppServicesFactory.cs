using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DntConsole.IoCConfig;

public static class AppServicesFactory
{
    public static void AddAppServices(this IServiceCollection services,
                                      IConfiguration configuration,
                                      bool useInMemoryDb = false)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSettings(configuration);
        services.AddHttpClient(configuration);
        services.AddCustomServices();
        services.AddDbContext(useInMemoryDb);
    }
}