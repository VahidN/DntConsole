using CommandLine;

namespace DntConsole.Models;

public class AppArgs
{
    [Option('i', nameof(Id), Required = false, HelpText = "App's ID")]
    public string? Id { get; set; }

    [Option('t', nameof(Token), Required = true, HelpText = "App's AccessToken")]
    public required string Token { get; set; }
}