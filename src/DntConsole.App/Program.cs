using DntConsole.IoCConfig;
using DntConsole.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
                  .ConfigureServices((context, services) => services.AddAppServices(context.Configuration));
var app = builder.Build();
await app.Services.GetRequiredService<IAppRunnerService>().StartAsync(args);