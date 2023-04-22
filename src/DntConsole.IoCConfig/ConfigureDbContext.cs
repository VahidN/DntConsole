using DntConsole.DataLayer.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DntConsole.IoCConfig;

public static class ConfigureDbContext
{
    public static void AddDbContext(this IServiceCollection services, bool useInMemoryDb)
    {
        services.AddDbContextPool<AppDbContext>((serviceProvider, optionsBuilder) =>
                                                {
                                                    if (useInMemoryDb)
                                                    {
                                                        optionsBuilder.UseInMemoryDb();
                                                    }
                                                    else
                                                    {
                                                        optionsBuilder.UseRealDb(serviceProvider);
                                                    }
                                                });
    }

    private static void UseRealDb(this DbContextOptionsBuilder optionsBuilder, IServiceProvider serviceProvider)
    {
        var connectionString = serviceProvider
                               .GetRequiredService<IAppDbContextFactory>()
                               .GetConnectionString();
        optionsBuilder.UseSqlite(connectionString);
    }

    /// <summary>
    ///     We are using this option for out integration tests
    /// </summary>
    [SuppressMessage("Microsoft.Usage",
                        "CA2000:Call System.IDisposable.Dispose on object created by 'new SqliteConnection(builder.ConnectionString)' before all references to it are out of scope",
                        Justification = "We need to keep the in-memory db alive!.")]
    private static void UseInMemoryDb(this DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new SqliteConnectionStringBuilder
                      {
                          DataSource = $"{Guid.NewGuid()}.db",
                          Mode = SqliteOpenMode.Memory,
                          Cache = SqliteCacheMode.Shared,
                      };
        var connection = new SqliteConnection(builder.ConnectionString);
        /*
        When the connection is opened, a new database is created in memory. 
        This database is destroyed when the connection is closed. 
        This means, we must keep the connection open until the tests ends.  
        Thus, the database won't be destroyed during the execution of the tests.
         */
        connection.Open();
        connection.EnableExtensions();
        optionsBuilder.UseSqlite(connection);
    }
}