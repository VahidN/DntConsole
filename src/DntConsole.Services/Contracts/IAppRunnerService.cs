namespace DntConsole.Services.Contracts;

public interface IAppRunnerService
{
    Task StartAsync(string[] args);
}