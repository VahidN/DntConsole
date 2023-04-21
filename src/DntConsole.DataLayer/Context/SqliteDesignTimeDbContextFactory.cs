using DntConsole.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DntConsole.DataLayer.Context;

public class SqliteDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
                          .ConfigureServices((context, services) =>
                                             {
                                                 services.AddSingleton<IAppDbContextFactory, AppDbContextFactory>();
                                                 services.Configure<AppConfig>(options =>
                                                                                   context.Configuration.Bind(options));
                                             });
        var app = builder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(app.Services.GetRequiredService<IAppDbContextFactory>().GetConnectionString());
        return new AppDbContext(optionsBuilder.Options);
    }
}